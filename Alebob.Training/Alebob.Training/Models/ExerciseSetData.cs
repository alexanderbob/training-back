using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.Models
{
    public class ExerciseSetData
    {
        public int Repetitions {get; set;}
        public float Weight { get; set; }

        public ExerciseSetData(float weight, int reps)
        {
            Weight = weight;
            Repetitions = reps;
        }
    }
}
