using Alebob.Training.DataLayer;
using Alebob.Training.Models;
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
        private IExerciseProvider _exerciseProvider;
        public ExercisesController(ILogger<ExercisesController> logger, IExerciseProvider dataProvider)
        {
            _logger = logger;
            _exerciseProvider = dataProvider;
        }

        [HttpGet("/api/[controller]/list")]
        public IEnumerable<ExerciseMetadata> AvailableExercises()
        {
            return _exerciseProvider.GetAvailableExercises();
        }

        [HttpGet("/api/[controller]/{isoDate}")]
        public ActionResult Exercises([FromRoute] string isoDate)
        {
            try
            {
                return Ok(_exerciseProvider.GetExercises(isoDate));
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
        public ActionResult SetExercise([FromRoute] string isoDate, [FromRoute] string exerciseCode, [FromBody] IEnumerable<ExerciseSetData> setsData)
        {
            try
            {
                _exerciseProvider.SetExercise(isoDate, exerciseCode, setsData);
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
