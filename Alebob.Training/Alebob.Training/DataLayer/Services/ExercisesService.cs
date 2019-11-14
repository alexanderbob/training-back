using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alebob.Training.DataLayer.Models;
using MongoDB.Driver;

namespace Alebob.Training.DataLayer.Services
{
    public class ExercisesService : IExerciseProvider
    {
        private readonly IMongoCollection<ExerciseMetadata> _exercisesCollection;
        public ExercisesService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _exercisesCollection = database.GetCollection<ExerciseMetadata>(settings.ExerciseMetadataCollectionName);
        }

        public async Task<IEnumerable<ExerciseMetadata>> GetExercises()
        {
            return await _exercisesCollection
                .Find(FilterDefinition<ExerciseMetadata>.Empty)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
