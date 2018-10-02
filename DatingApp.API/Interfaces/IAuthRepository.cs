using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Interfaces
{
    public interface IAuthRepository
    {
         Task<User> Register(string username, string password);

         Task<User> LoginAsync(string username, string password);

         Task<bool> UserExists(string username);
    }
}