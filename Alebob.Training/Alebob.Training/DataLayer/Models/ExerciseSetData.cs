using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models
{
    public class ExerciseSetData
    {
        [BsonElement("repetitions")]
        public int Repetitions { get; set; }

        [BsonElement("weight")]
        public decimal Weight { get; set; }

        public ExerciseSetData()
        {

        }
        public ExerciseSetData(decimal weight, int reps)
        {
            Weight = weight;
            Repetitions = reps;
        }
    }
}
