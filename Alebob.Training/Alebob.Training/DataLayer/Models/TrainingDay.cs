using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models
{
    public class TrainingDay : TrainingDayMetadata
    {
        [BsonElement(Const.TrainingExerciseKey)]
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.Document)]
        public Dictionary<string, List<ExerciseSetData>> Exercises { get; set; }
    }
}
