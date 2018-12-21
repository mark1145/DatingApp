using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int _maxPagePageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get
            { return _pageSize; }
            set
            {
                _pageSize = (value > _maxPagePageSize) ? _maxPagePageSize : value;
            }
        }

        public int UserId { get; set; }

        public string Gender { get; set; }

        public int? MinAge { get; set; }

        public int? MaxAge { get; set; }

        public string OrderBy { get; set; }

        public bool Likees { get; set; } = false;

        public bool Likers { get; set; } = false;

    }
}


