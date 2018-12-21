using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> IsUserAlreadyLikedAsync(int userId, int recipientId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="likers">True will fetch all people liked by this person. False will fetch all people that like this person</param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetUserLikes(int id, bool likers);
    }
}
