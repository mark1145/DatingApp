using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserAsync(int id);

        Task<User> GetUserPhotoAsync(int id);

        Task<PageList<User>> GetUsersAsync(UserParams userParams);


    }
}
