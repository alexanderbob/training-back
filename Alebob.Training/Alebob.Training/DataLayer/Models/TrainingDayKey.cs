using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models
{
    public class TrainingDayKey
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; }
        /// <summary>
        /// Date stored in yyyy-MM-dd format
        /// </summary>
        [BsonElement("date")]
        public string Date { get; set; }

        public TrainingDayKey()
        {

        }

        public TrainingDayKey(string userId, string date)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (string.IsNullOrWhiteSpace(date))
            {
                throw new ArgumentNullException(nameof(date));
            }
            DateTime dt;
            if (!DateTime.TryParseExact(
                date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dt))
            {
                throw new ArgumentException($"Provided date {date} is not in 'yyyy-MM-dd' format", nameof(date));
            }
            UserId = userId;
            Date = date;
        }
    }
}
