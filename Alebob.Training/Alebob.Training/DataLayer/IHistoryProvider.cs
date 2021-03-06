﻿using Alebob.Training.DataLayer.Models;
using Alebob.Training.DataLayer.Models.TimeSeries;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer
{
    public interface IHistoryProvider
    {
        public Task<IEnumerable<ExerciseHistoricalItem>> GetHistory(string userId, string exerciseCode);
        public Task<Dictionary<string, TrainingDayMetadata>> GetHistory(string userId, int limit);
        public Task<Dictionary<string, TrainingDayMetadata>> GetHistory(string userId, string isoStartDate, string isoEndDate);
        public Task<TrainingDay> GetEntry(TrainingDayKey key);
        public Task<long> UpdateEntry(TrainingDayKey key, string name, string description);
        public Task<TrainingDayMetadata> AllocateEntry(TrainingDayKey key, string name, string description);
        public Task<long> UpsertExercise(
            TrainingDayKey key, string exerciseCode, IEnumerable<ExerciseSetData> data
        );
    }
}
