using System;

namespace DatingApp.API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsDeletedBySender { get; set; }
        public bool IsDeletedByRecipient { get; set; }

        public User Sender { get; set; }
        public User Recipient { get; set; }

        // lazy loading
        // public virtual User Sender { get; set; }
        // public virtual User Recipient { get; set; }
    }
}