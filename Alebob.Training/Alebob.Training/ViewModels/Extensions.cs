﻿using Alebob.Training.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public static class Extensions
    {
        public static HistoryEntry AsViewModel(this TrainingDayMetadata metadata)
        {
            return new HistoryEntry
            {
                IsoDate = metadata.Id.Date,
                SubTitle = metadata.Description,
                Title = metadata.Name
            };
        }

        public static Dictionary<string, ExerciseEntry> AsViewModel(this TrainingDay day, Dictionary<string, ExerciseMetadata> exercises)
        {
            return day.Exercises.ToDictionary(
                x => x.Key,
                x => new ExerciseEntry
                {
                    Metadata = exercises[x.Key],
                    Sets = x.Value.Select(y => new ExerciseSetData(decimal.ToDouble(y.Weight), y.Repetitions))
                }
            );
        }

        public static ExerciseMetadata AsViewModel(this DataLayer.Models.ExerciseMetadata metadata)
        {
            return new ExerciseMetadata(metadata.Code, metadata.Description);
        }

        public static ExerciseHistoricalItem AsViewModel(this DataLayer.Models.TimeSeries.ExerciseHistoricalItem item)
        {
            var data = item.Value;
            return new ExerciseHistoricalItem
            {
                Date = item.Observation,
                Repetitions = new MinMaxAvgItem(data.Repetitions.Min, data.Repetitions.Max, data.Repetitions.Average),
                Tonnage = data.Tonnage,
                Weight = new MinMaxAvgItem(data.Weight.Min, data.Weight.Max, data.Weight.Average)
            };
        }

        public static DataLayer.Models.ExerciseSetData AsDataModel(this ViewModels.ExerciseSetData setData)
        {
            return new DataLayer.Models.ExerciseSetData(Convert.ToDecimal(setData.Weight), setData.Repetitions);
        }
    }
}
