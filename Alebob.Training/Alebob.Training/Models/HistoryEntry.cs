using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.Models
{
    public class HistoryEntry : EntryWithTitle
    {
        public DateTime Date { get; set; }
        public string SubTitle { get; set; }
    }
}
