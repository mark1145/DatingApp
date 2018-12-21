using DatingApp.API.Data;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        public readonly DataContext _repository;

        public PhotoRepository(DataContext _dataContext)
        {
            _repository = _dataContext;
            Console.WriteLine(_dataContext.GetHashCode() + " photoRepo");
        }

        public void DeletePhoto(Photo photo)
        {
            _repository.Remove(photo);
        }

        public async Task<Photo> GetMainPhotoForUserAsync(int userId)
        {
            return await _repository.Photos.Where(x => x.UserId == userId).FirstOrDefaultAsync(y => y.IsMain);
        }

        public async Task<Photo> GetPhotoAsync(int photoId)
        {
            Photo photo = await _repository.Photos.FirstOrDefaultAsync(x => x.Id == photoId);

            return photo;
        }

        public async Task<bool> SaveAll()
        {
            return await _repository.SaveChangesAsync() > 0;
        }
    }
}
