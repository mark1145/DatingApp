using DatingApp.API.Data;
using DatingApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _dataContext;

        public LikeRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> IsUserAlreadyLikedAsync(int userId, int recipientId)
        {
            var ret = await _dataContext.Likes.AnyAsync(x => x.LikerId == userId && x.LikeeId == recipientId);

            return ret;
        }

        public async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var likes = await _dataContext.Likes.Where(x => x.LikeeId == id || x.LikerId == id).ToListAsync();

            if (likers)
                return likes.Select(x => x.LikerId);

            return likes.Select(x => x.LikeeId);
        }
    }
}
