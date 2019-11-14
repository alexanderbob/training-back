using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string ExerciseMetadataCollectionName { get; set; }
        string TrainingCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string DatabaseName { get; set; }
    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string ExerciseMetadataCollectionName { get; set; }
        public string TrainingCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }
}
