using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models
{
    public class ExerciseMetadata
    {
        [BsonId]
        public string Code { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }

        public ExerciseMetadata() { }
        public ExerciseMetadata(string code, string description)
        {
            Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentNullException(nameof(code)) : code;
            Description = string.IsNullOrWhiteSpace(description) ? throw new ArgumentNullException(nameof(description)) : description;
        }
    }
}
