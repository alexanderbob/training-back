using Alebob.Training.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.DataLayer
{
    public interface IUserProvider
    {
        Task<User> FindUser(string email);
        Task<User> UpsertUser(string email, string displayName);
    }
}
