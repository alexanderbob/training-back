using Alebob.Training.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer
{
    public interface IHistoryProvider
    {
        public Dictionary<string, HistoryEntry> GetHistory();
        public HistoryEntry GetEntry(string isoDate);
        public HistoryEntry PutEntry(HistoryEntry entry);
    }
}
