using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    public class MessageToReturnDto
    {

        public MessageToReturnDto()
        {

        }

        public MessageToReturnDto(Message x)
        {
            Id = x.Id;
            Content = x.Content;
            DateRead = x.DateRead;
            IsRead = x.IsRead;
            MessageSent = x.MessageSent;
            RecipientId = x.RecipientId;
            if (x.Recipient != null)
            {
                RecipientKnownAs = x.Recipient.KnownAs;
                if (x.Recipient.Photos != null)
                    RecipientPhotoUrl = x.Recipient.Photos.FirstOrDefault(y => y.IsMain).Url;
            }
            
            SenderId = x.SenderId;

            if (x.Sender != null)
            {
                SenderKnownAs = x.Sender.KnownAs;
                if (x.Sender.Photos != null)
                   SenderPhotoUrl = x.Sender.Photos.FirstOrDefault(y => y.IsMain).Url;
            }
        }

        public int Id { get; set; }

        public int SenderId { get; set; } //Automapper is able to infer this value from the User class becasue it has a property with that name

        public string SenderKnownAs { get; set; } //Automapper is able to infer this value from the User class becasue it has a property with that name

        public string SenderPhotoUrl { get; set; } //Automapper is able to infer this value from the User class becasue it has a property with that name

        public int RecipientId { get; set; } //Automapper is able to infer this value from the User class becasue it has a property with that name

        public string RecipientKnownAs { get; set; } //Automapper is able to infer this value from the User class becasue it has a property with that name

        public string RecipientPhotoUrl { get; set; } //Automapper is able to infer this value from the User class becasue it has a property with that name

        public string Content { get; set; }

        public bool IsRead { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; }
    }
}
