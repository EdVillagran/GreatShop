using Microsoft.AspNetCore.Identity;

namespace GreatShop.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }

    }
}
