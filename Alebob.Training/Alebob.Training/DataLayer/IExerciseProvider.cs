﻿using Alebob.Training.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer
{
    public interface IExerciseProvider
    {
        Task<IEnumerable<ExerciseMetadata>> GetExercises();
    }
}
