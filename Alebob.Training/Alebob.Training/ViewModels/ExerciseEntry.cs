﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.ViewModels
{
    public class ExerciseEntry
    {
        public ExerciseMetadata Metadata { get; set; }
        public IEnumerable<ExerciseSetData> Sets { get; set; }
    }
}
