using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alebob.Training.Models;

namespace Alebob.Training.DataLayer
{
    public class ExerciseProvider : IExerciseProvider
    {
        private List<ExerciseMetadata> _exerciseMetadata;
        //Dictionary<isoDate, Dictionary<exerciseCode, ExerciseEntry>>
        private Dictionary<string, Dictionary<string, ExerciseEntry>> _exerciseHistory;

        public ExerciseProvider(IHistoryProvider historyProvider)
        {
            _exerciseMetadata = new List<ExerciseMetadata>
            {
                new ExerciseMetadata("squats", "Squats"),
                new ExerciseMetadata("deadlift", "Dead Lift"),
                new ExerciseMetadata("benchpress", "Bench Press"),
                new ExerciseMetadata("legpress", "Leg Press"),
                new ExerciseMetadata("longpull", "Long Pull"),
                new ExerciseMetadata("frenchpress", "French Press"),
                new ExerciseMetadata("bicepsbench", "Biceps Bench"),
                new ExerciseMetadata("bicepsdumbbell", "Biceps Dumbbell"),
                new ExerciseMetadata("tricepsbarbell", "Triceps Barbell"),
                new ExerciseMetadata("tricepspull", "Triceps Pull")
            };
            
            _exerciseHistory = new Dictionary<string, Dictionary<string, ExerciseEntry>>();
            var random = new Random();
            var history = historyProvider.GetHistory();
            foreach (var kvp in history)
            {
                _exerciseHistory[kvp.Key] = new Dictionary<string, ExerciseEntry>();
                int totalExercises = random.Next(0, _exerciseMetadata.Count);
                for (int i = 0; i < totalExercises; i++)
                {
                    int sets = random.Next(1, 7);
                    var setsData = new ExerciseSetData[sets];
                    for (int j = 0; j < sets; j++)
                    {
                        setsData[j] = new ExerciseSetData(
                            Math.Round(100 * random.NextDouble(), 1),
                            random.Next(1, 20)
                        );
                    }
                    _exerciseHistory[kvp.Key][_exerciseMetadata[i].Code] = new ExerciseEntry
                    {
                        Metadata = _exerciseMetadata[i],
                        Sets = setsData
                    };
                }
            }
        }

        public IEnumerable<ExerciseMetadata> GetAvailableExercises()
        {
            return _exerciseMetadata;
        }

        public Dictionary<string, ExerciseEntry> GetExercises(string isoDate)
        {
            DateTime.ParseExact(isoDate, "yyyy-MM-dd", null);
            return _exerciseHistory[isoDate];
        }

        public ExerciseEntry SetExercise(string isoDate, string exerciseCode, IEnumerable<ExerciseSetData> setsData)
        {
            if (!_exerciseHistory.ContainsKey(isoDate))
            {
                throw new ArgumentException($"No training day on provided date {isoDate}", nameof(isoDate));
            }
            var exercise = _exerciseMetadata.FirstOrDefault(x => x.Code == exerciseCode);
            if (exercise == null)
            {
                throw new ArgumentException($"No exercise with code {exerciseCode}", nameof(exerciseCode));
            }
            var entry = new ExerciseEntry
            {
                Metadata = exercise,
                Sets = setsData
            };
            _exerciseHistory[isoDate][exerciseCode] = entry;
            return entry;
        }

        public void AllocateTrainingDay(string isoDate)
        {
            if (_exerciseHistory.ContainsKey(isoDate))
            {
                throw new ArgumentException($"Training date {isoDate} is already created", nameof(isoDate));
            }
            _exerciseHistory[isoDate] = new Dictionary<string, ExerciseEntry>();
        }
    }
}
