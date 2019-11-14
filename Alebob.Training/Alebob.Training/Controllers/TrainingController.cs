using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alebob.Training.DataLayer;
using Alebob.Training.OAuth;
using Alebob.Training.ViewModels;
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
        private IHistoryProvider _historyProvider;
        private IExerciseProvider _exercisesProvider;

        public TrainingController(
            ILogger<TrainingController> logger,
            IHistoryProvider historyEntries,
            IExerciseProvider exerciseProvider
        )
        {
            _logger = logger;
            _historyProvider = historyEntries;
            _exercisesProvider = exerciseProvider;
        }

        [HttpGet]
        public async Task<IEnumerable<HistoryEntry>> Get()
        {
            return (await _historyProvider
                .GetHistory(User.FindFirst(Claims.UserId).Value, 15)
                .ConfigureAwait(false))
                .Select(x => x.Value.AsViewModel())
                .ToList();
        }

        [HttpGet("/api/[controller]/range/from/{startIsoDate}/to/{endIsoDate}")]
        public async Task<IEnumerable<HistoryEntry>> Range([FromRoute] string startIsoDate, [FromRoute] string endIsoDate)
        {
            return (await _historyProvider
                .GetHistory(User.FindFirst(Claims.UserId).Value, startIsoDate, endIsoDate)
                .ConfigureAwait(false))
                .Select(x => x.Value.AsViewModel())
                .ToList();
        }

        [HttpPost]
        public async Task<ActionResult> Post(HistoryEntry entry)
        {
            var key = new DataLayer.Models.TrainingDayKey(
                User.FindFirst(Claims.UserId).Value, entry.IsoDate
            );
            await _historyProvider.AllocateEntry(key, entry.Title, entry.SubTitle).ConfigureAwait(false);
            return Ok();
        }
    }
}