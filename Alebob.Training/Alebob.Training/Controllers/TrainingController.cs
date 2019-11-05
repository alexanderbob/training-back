using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alebob.Training.DataLayer;
using Alebob.Training.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Alebob.Training.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private ILogger<TrainingController> _logger;
        private IHistoryProvider _historyEntries;

        public TrainingController(
            ILogger<TrainingController> logger,
            IHistoryProvider historyEntries
        )
        {
            _logger = logger;
            _historyEntries = historyEntries;
        }

        [HttpGet]
        public IEnumerable<HistoryEntry> Get()
        {
            return _historyEntries.GetHistory().Values;
        }

        [HttpPost]
        public ActionResult Post(HistoryEntry entry)
        {
            _historyEntries.PutEntry(entry);
            return Ok();
        }
    }
}