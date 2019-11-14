using Alebob.Training.DataLayer;
using Alebob.Training.DataLayer.Models;
using Alebob.Training.OAuth;
using Alebob.Training.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private ILogger<ExercisesController> _logger;
        private IHistoryProvider _historyProvider;
        private IExerciseProvider _exerciseProvider;
        public ExercisesController(ILogger<ExercisesController> logger, IHistoryProvider dataProvider, IExerciseProvider exerciseProvider)
        {
            _logger = logger;
            _historyProvider = dataProvider;
            _exerciseProvider = exerciseProvider;
        }

        [HttpGet("/api/[controller]/list")]
        public async Task<IEnumerable<ViewModels.ExerciseMetadata>> AvailableExercises()
        {
            return (await _exerciseProvider.GetExercises().ConfigureAwait(false))
                .Select(x => x.AsViewModel());
        }

        [HttpGet("/api/[controller]/{isoDate}")]
        public async Task<ActionResult> Exercises([FromRoute] string isoDate)
        {
            try
            {
                var key = new TrainingDayKey(User.FindFirst(Claims.UserId).Value, isoDate);
                //TODO: return dictionary from IExerciseProvider
                var exercises = 
                    (await _exerciseProvider.GetExercises().ConfigureAwait(false))
                    .ToDictionary(x => x.Code, y => y.AsViewModel());
                var entry = await _historyProvider.GetEntry(key).ConfigureAwait(false);
                return Ok(entry.AsViewModel(exercises));
            }
            catch (Exception exc)
            {
                if (exc is KeyNotFoundException)
                {
                    return NotFound();
                }
                if (exc is FormatException || exc is ArgumentNullException)
                {
                    return BadRequest();
                }
                throw exc;
            }
        }

        [HttpPost("/api/[controller]/{isoDate}/{exerciseCode}")]
        public async Task<ActionResult> SetExercise([FromRoute] string isoDate, [FromRoute] string exerciseCode, [FromBody] IEnumerable<ViewModels.ExerciseSetData> setsData)
        {
            try
            {
                var key = new TrainingDayKey(User.FindFirst(Claims.UserId).Value, isoDate);
                await _historyProvider.UpsertExercise(key, exerciseCode, setsData.Select(x => x.AsDataModel())).ConfigureAwait(false);
            }
            catch (ArgumentException exc)
            {
                _logger.LogError(exc, "Faced exception during SetExercise routine", new object[] { isoDate, exerciseCode, setsData });
                return BadRequest();
            }
            return Ok();
        }
    }
}
