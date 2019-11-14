using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string DisplayName { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("lastSignIn")]
        public BsonTimestamp LastSignIn { get; set; }
    }
}
