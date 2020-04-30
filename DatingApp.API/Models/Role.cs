using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Models
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRole { get; set; }

        // lazy loading
        // public virtual ICollection<UserRole> UserRole { get; set; }
    }
}