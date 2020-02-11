using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public class ExerciseHistoricalItem
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("tonnage")]
        public double Tonnage { get; set; }
        [JsonPropertyName("weight")]
        public MinMaxAvgItem Weight { get; set; }
        [JsonPropertyName("repetitions")]
        public MinMaxAvgItem Repetitions { get; set; }

        public ExerciseHistoricalItem() { }
        public ExerciseHistoricalItem(string date, double tonnage, MinMaxAvgItem weight, MinMaxAvgItem reps)
        {
            Date = date;
            Tonnage = tonnage;
            Weight = weight;
            Repetitions = reps;
        }
    }
}
