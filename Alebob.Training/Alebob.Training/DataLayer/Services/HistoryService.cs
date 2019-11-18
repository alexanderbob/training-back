using Alebob.Training.DataLayer.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Services
{
    public class HistoryService : IHistoryProvider
    {
        private readonly IMongoCollection<TrainingDay> _historyEntries;

        public HistoryService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _historyEntries = database.GetCollection<TrainingDay>(settings.TrainingCollectionName);
        }

        public async Task<TrainingDayMetadata> AllocateEntry(TrainingDayKey key, string name, string description)
        {
            ValidateKey(key);
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }
            var doc = new TrainingDay
            {
                Id = key,
                Description = description,
                Exercises = new Dictionary<string, List<ExerciseSetData>>(),
                Name = name
            };
            await _historyEntries.InsertOneAsync(doc).ConfigureAwait(false);
            return doc;
        }

        public async Task<long> UpdateEntry(TrainingDayKey key, string name, string description)
        {
            ValidateKey(key);
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }
            var filter = Builders<TrainingDay>.Filter.Eq(x => x.Id, key);
            var update = Builders<TrainingDay>.Update
                .Set(x => x.Name, name)
                .Set(x => x.Description, description);
            var result = await _historyEntries.UpdateOneAsync(filter, update).ConfigureAwait(false);
            return result.ModifiedCount;
        }

        public async Task<TrainingDay> GetEntry(TrainingDayKey key)
        {
            var filter = Builders<TrainingDay>.Filter.Eq(x => x.Id, key);
            var result = await _historyEntries
                .Find(filter)
                .FirstAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<Dictionary<string, TrainingDayMetadata>> GetHistory(string userId, string isoStartDate, string isoEndDate)
        {
            ValidateUserId(userId);
            ValidateDateIso(isoStartDate);
            ValidateDateIso(isoEndDate);
            var fb = Builders<TrainingDay>.Filter;
            var filter = Builders<TrainingDay>.Filter
                .And(
                    fb.Eq(x => x.Id.UserId, userId),
                    fb.Lt(x => x.Id.Date, isoEndDate),
                    fb.Gte(x => x.Id.Date, isoStartDate)
                );
            var project = Builders<TrainingDay>.Projection
                .Exclude(x => x.Exercises);
            Dictionary<string, TrainingDayMetadata> result = new Dictionary<string, TrainingDayMetadata>();
            await _historyEntries
                .Find(filter)
                .Project<TrainingDayMetadata>(project)
                .SortByDescending(x => x.Id.Date)
                .ForEachAsync(x =>
                {
                    result[x.Id.Date] = x;
                }).ConfigureAwait(false);
            return result;
        }

        public async Task<Dictionary<string, TrainingDayMetadata>> GetHistory(string userId, int limit)
        {
            ValidateUserId(userId);
            var filter = Builders<TrainingDay>.Filter
                .Eq(x => x.Id.UserId, userId);
            var project = Builders<TrainingDay>.Projection
                .Exclude(x => x.Exercises);
            Dictionary<string, TrainingDayMetadata> result = new Dictionary<string, TrainingDayMetadata>();
            await _historyEntries
                .Find(filter)
                .Project<TrainingDayMetadata>(project)
                .SortByDescending(x => x.Id.Date)
                .Limit(limit)
                .ForEachAsync(x => 
                {
                    result[x.Id.Date] = x;
                }).ConfigureAwait(false);
            return result;
        }

        public async Task<long> UpsertExercise(TrainingDayKey key, string exerciseCode, IEnumerable<ExerciseSetData> data)
        {
            ValidateKey(key);
            var filter = Builders<TrainingDay>.Filter
                .Eq(x => x.Id, key);
            var update = Builders<TrainingDay>.Update
                .Set($"{Const.TrainingExerciseKey}.{exerciseCode}", data);
            var result = await _historyEntries
                .UpdateOneAsync(filter, update, new UpdateOptions
                {
                    IsUpsert = true
                }).ConfigureAwait(false);
            return result.ModifiedCount;
        }

        private void ValidateKey(TrainingDayKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            ValidateUserId(key.UserId);
            ValidateDateIso(key.Date);
        }

        private void ValidateUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            ObjectId id;
            if (!ObjectId.TryParse(userId, out id))
            {
                throw new ArgumentException($"Provided user id {userId} in key is not Mongo ObjectId");
            }
        }

        private void ValidateDateIso(string dateIso)
        {
            if (string.IsNullOrWhiteSpace(dateIso))
            {
                throw new ArgumentNullException(nameof(dateIso));
            }
            DateTime dt;
            if (!DateTime.TryParseExact(dateIso, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                throw new ArgumentException($"Provided date {dateIso} in key is not in 'yyyy-MM-dd' format");
            }
        }
    }
}
