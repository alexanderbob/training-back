using Alebob.Training.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public class HistoryEntry : EntryWithTitle
    {
        [JsonPropertyName("date")]
        public string IsoDate { get; set; }
        public string SubTitle { get; set; }
    }
}
