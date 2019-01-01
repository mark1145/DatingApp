using DatingApp.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class MessageParams
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

        public MessageContainerEnum MessageContainer { get; set; }
    }
}
