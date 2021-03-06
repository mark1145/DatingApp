﻿using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        //TODO : Abstract Id of profilePhoto to a new table; possibly like; Id PhotoId UserId
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }

    }
}