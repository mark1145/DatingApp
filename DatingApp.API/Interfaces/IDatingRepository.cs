using DatingApp.API.Helpers;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    /// <summary>
    /// For interacting with both the Users and Photos tables
    /// </summary>
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<PageList<User>> GetUsers(UserParams userParams);

        Task<User> GetUser(int id);
    }
}
