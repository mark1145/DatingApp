using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    public interface IPhotosHosting
    {
        Task<Photo> StorePhoto(PhotoForCreationDto photoForCreationDto);

        Task<bool> DeletePhoto(Photo photo);
    }
}
