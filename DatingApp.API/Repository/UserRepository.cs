using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILikeRepository _likeRepository;

        public UserRepository(DataContext dataContext, ILikeRepository likeRepository)
        {
            _dataContext = dataContext;
            _likeRepository = likeRepository;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(y => y.Id == id);

            return user;
        }

        public async Task<User> GetUserPhotoAsync(int id)
        {
            var user = await _dataContext.Users.Include(x => x.Photos).FirstOrDefaultAsync(y => y.Id == id);

            return user;
        }

        public async Task<PageList<User>> GetUsersAsync(UserParams userParams)
        {
            var users = _dataContext.Users
                .Include(x => x.Photos)
                .OrderByDescending(x => x.LastActive)
                .AsQueryable();

            users = users.Where(x => x.Id != userParams.UserId);

            if (userParams.Likees)
            {
                var ids = await _likeRepository.GetUserLikes(userParams.UserId, false);
                users = users.Where(x => ids.Contains(x.Id));
            }

            if (userParams.Likers)
            {
                var ids = await _likeRepository.GetUserLikes(userParams.UserId, true);
                users = users.Where(x => ids.Contains(x.Id));
            }

            if (userParams.Gender.ToLower() != "both")
                users = users.Where(x => x.Gender == userParams.Gender);

            if (userParams.MinAge.HasValue)
            {
                DateTime maxDob = DateTime.Today.AddYears(-userParams.MinAge.Value);
                users = users.Where(x => x.DateOfBirth <= maxDob);
            }

            if (userParams.MaxAge.HasValue)
            {
                DateTime minDob = DateTime.Today.AddYears(-userParams.MaxAge.Value - 1);
                users = users.Where(x => x.DateOfBirth >= minDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy.ToLower())
                {
                    case "created":
                        users = users.OrderByDescending(x => x.Created);
                        break;
                    case "oldestfirst":
                        users = users.OrderBy(x => x.DateOfBirth);
                        break;
                    case "youngestfirst":
                        users = users.OrderByDescending(x => x.DateOfBirth);
                        break;
                    default:
                        users = users.OrderByDescending(x => x.LastActive);
                        break;

                }
            }


            return await PageList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
    }
}
