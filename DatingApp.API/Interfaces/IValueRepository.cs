using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Interfaces
{
    public interface IValueRepository
    {
         Task<List<Value>> GetValuesAsync();

         Task<Value> GetValueAsync(int id);
    }
}