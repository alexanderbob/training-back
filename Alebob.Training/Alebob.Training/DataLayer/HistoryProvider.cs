using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alebob.Training.Models;

namespace Alebob.Training.DataLayer
{
    public class HistoryProvider : IHistoryProvider
    {
        private Dictionary<string, HistoryEntry> _entries;
        public HistoryProvider()
        {
            _entries = new Dictionary<string, HistoryEntry>();
            DateTime now = DateTime.Now;
            for (int i = 0; i < 15; i++)
            {
                _entries[now.ToString("yyyy-MM-dd")] = new HistoryEntry
                {
                    Date = now,
                    Title = now.ToString("D"),
                    SubTitle = $"Exercise on {now.ToLongDateString()}"
                };
                now = now.Subtract(TimeSpan.FromDays(1));
            }
        }
        public Dictionary<string, HistoryEntry> GetHistory()
        {
            return _entries;
        }

        public HistoryEntry GetEntry(string isoDate)
        {
            HistoryEntry value;
            _entries.TryGetValue(isoDate, out value);
            return value;
        }

        public HistoryEntry PutEntry(HistoryEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            _entries[entry.Date.ToString("yyyy-MM-dd")] = entry;
            return entry;
        }
    }
}
