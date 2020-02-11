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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _historyProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _exerciseProvider = exerciseProvider ?? throw new ArgumentNullException(nameof(exerciseProvider));
        }

        [HttpGet("/api/[controller]/list")]
        public async Task<IEnumerable<ViewModels.ExerciseMetadata>> AvailableExercises()
        {
            return (await _exerciseProvider.GetExercises().ConfigureAwait(false))
                .Select(x => x.AsViewModel());
        }

        [HttpGet("/api/[controller]/history/{exerciseCode}")]
        public async Task<IEnumerable<ViewModels.ExerciseHistoricalItem>> History([FromRoute] string exerciseCode)
        {
            var userId = User.FindFirst(Claims.UserId).Value;
            return (
                await _historyProvider
                        .GetHistory(userId, exerciseCode)
                        .ConfigureAwait(false)
            ).Select(x => x.AsViewModel());
        }

        [HttpGet("/api/[controller]/{isoDate}")]
        public async Task<ActionResult> Exercises([FromRoute] string isoDate)
        {
            try
            {
                var key = new TrainingDayKey(User.FindFirst(Claims.UserId).Value, isoDate);
                //TODO: cache exercises
                var exercises = (await _exerciseProvider.GetExercises().ConfigureAwait(false))
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
                var result = await _historyProvider.UpsertExercise(key, exerciseCode, setsData.Select(x => x.AsDataModel())).ConfigureAwait(false);

                var exercises = (await _exerciseProvider.GetExercises().ConfigureAwait(false))
                    .ToDictionary(x => x.Code, y => y.AsViewModel());
                var training = await _historyProvider.GetEntry(key).ConfigureAwait(false);
                var description = string.Join(", ", exercises.Where(x => training.Exercises.ContainsKey(x.Key)).Select(x => x.Value.Description));
                await _historyProvider.UpdateEntry(key, training.Name, description).ConfigureAwait(false);
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
