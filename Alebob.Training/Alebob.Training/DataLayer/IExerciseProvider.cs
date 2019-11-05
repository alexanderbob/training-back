using Alebob.Training.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer
{
    public interface IExerciseProvider
    {
        IEnumerable<ExerciseMetadata> GetAvailableExercises();
        Dictionary<string, ExerciseEntry> GetExercises(string isoDate);
        ExerciseEntry SetExercise(string isoDate, string exerciseCode, IEnumerable<ExerciseSetData> entry);
    }
}
