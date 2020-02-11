using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public struct MinMaxAvgItem : IEquatable<MinMaxAvgItem>
    {
        [JsonPropertyName("min")]
        public double Min { get; set; }
        [JsonPropertyName("max")]
        public double Max { get; set; }
        [JsonPropertyName("average")]
        public double Average { get; set; }

        public MinMaxAvgItem(double min, double max, double average)
        {
            Min = min;
            Max = max;
            Average = average;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MinMaxAvgItem))
            {
                return false;
            }
            var a = (MinMaxAvgItem)obj;
            return Equals(a);
        }

        public override int GetHashCode()
        {
            return (Min + Max + Average).GetHashCode();
        }

        public static bool operator ==(MinMaxAvgItem left, MinMaxAvgItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MinMaxAvgItem left, MinMaxAvgItem right)
        {
            return !(left == right);
        }

        public bool Equals(MinMaxAvgItem a)
        {
            return Min == a.Min &&
                Max == a.Max &&
                Average == a.Average;
        }
    }
}
