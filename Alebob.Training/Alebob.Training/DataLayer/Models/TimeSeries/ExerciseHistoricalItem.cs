using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models.TimeSeries
{
    public class ExerciseHistoricalItem
    {
        [BsonId]
        public string Observation { get; set; }
        [BsonElement("value")]
        public ExerciseHistoricalValue Value { get; set; }
    }

    public class ExerciseHistoricalValue
    {
        [BsonElement("tonnage")]
        public double Tonnage { get; set; }
        [BsonElement("weight")]
        public MinMaxAvgItem Weight { get; set; }
        [BsonElement("reps")]
        public MinMaxAvgItem Repetitions { get; set; }
    }
}
