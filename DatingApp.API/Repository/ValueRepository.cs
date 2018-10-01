using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Repository
{
    public class ValueRepository : IValueRepository
    {
        private readonly DataContext _dataContext;
        
        public ValueRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Value>> GetValuesAsync()
        {
            List<Value> ret = await _dataContext.Values.ToListAsync();

            return ret;
        }

        public async Task<Value> GetValueAsync(int id)
        {
            Value ret = await _dataContext.Values.FirstOrDefaultAsync(a => a.Id == id);

            return ret;
        }
    }
}