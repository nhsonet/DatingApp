namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; }
        public int LikeeId { get; set; }

        public User Liker { get; set; }
        public User Likee { get; set; }

        // lazy loading
        // public virtual User Liker { get; set; }
        // public virtual User Likee { get; set; }
    }
}