using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public class ExerciseSetData
    {
        [JsonPropertyName("repetitions")]
        public int Repetitions { get; set; }

        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        public ExerciseSetData()
        {

        }
        public ExerciseSetData(double weight, int reps)
        {
            Weight = weight;
            Repetitions = reps;
        }
    }
}
