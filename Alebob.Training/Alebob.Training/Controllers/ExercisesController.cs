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
        private Task<Dictionary<string, DataLayer.Models.ExerciseMetadata>> _availableExercises;
        public ExercisesController(ILogger<ExercisesController> logger, IHistoryProvider dataProvider, IExerciseProvider exerciseProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _historyProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _exerciseProvider = exerciseProvider ?? throw new ArgumentNullException(nameof(exerciseProvider));
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            _exerciseProvider.GetExercises().ContinueWith(_ =>
            {
                _availableExercises = Task.FromResult(_.Result.ToDictionary(k => k.Code));
            });
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
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
                var exercises = (await _availableExercises.ConfigureAwait(false))
                    .ToDictionary(x => x.Value.Code, y => y.Value.AsViewModel());
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
                
                var exercises = await _availableExercises.ConfigureAwait(false);
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
