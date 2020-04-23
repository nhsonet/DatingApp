using System;

namespace DatingApp.API.DTOs
{
    public class MessageForAddDTO
    {
        public MessageForAddDTO()
        {
            SentAt = DateTime.Now;
        }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime SentAt { get; set; }
        public string Content { get; set; }
    }
}