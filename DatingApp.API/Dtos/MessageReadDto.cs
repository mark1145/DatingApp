﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    public class MessageReadDto
    {
        public IEnumerable<int> MessageIds { get; set; }
    }
}
