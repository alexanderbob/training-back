using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.Models
{
    public class ExerciseMetadata
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public ExerciseMetadata(string code, string description)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
