using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhotoAsync(int photoId);

        Task<Photo> GetMainPhotoForUserAsync(int userId);

        void DeletePhoto(Photo photo);

        Task<bool> SaveAll();
    }
}
