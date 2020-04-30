using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsMain { get; set; }
        public bool IsApproved { get; set; }
        public string PublicId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }

        // lazy loading
        //public virtual User User { get; set; }
    }
}