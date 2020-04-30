using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }

        // lazy loading
        // public virtual User User { get; set; }
        // public virtual Role Role { get; set; }
    }
}