using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alebob.Training.DataLayer.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Alebob.Training.DataLayer.Services
{
    public class UsersService : IUserProvider
    {
        private readonly IMongoCollection<User> _usersCollection;
        public UsersService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _usersCollection = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<User> FindUser(string email)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            var result = await _usersCollection
                .Find(filter)
                .FirstAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<User> UpsertUser(string email, string displayName)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            var update = Builders<User>.Update
                .Set(x => x.DisplayName, displayName)
                .CurrentDate(x => x.LastSignIn, UpdateDefinitionCurrentDateType.Timestamp);
            var result = await _usersCollection
                .UpdateOneAsync(filter, update, new UpdateOptions
                {
                    IsUpsert = true
                })
                .ConfigureAwait(false);
            
            var list = await _usersCollection.Find(filter).ToListAsync().ConfigureAwait(false);
            return list.First();
        }
    }
}
