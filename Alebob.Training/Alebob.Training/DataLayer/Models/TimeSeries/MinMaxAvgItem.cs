using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer.Models.TimeSeries
{
    public class MinMaxAvgItem
    {
        [BsonElement("min")]
        public double Min { get; set; }
        [BsonElement("max")]
        public double Max { get; set; }
        [BsonElement("avg")]
        public double Average { get; set; }
    }
}
