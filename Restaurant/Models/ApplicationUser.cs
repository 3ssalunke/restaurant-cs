using Microsoft.AspNetCore.Identity;

namespace Restaurant.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order>? Orders { get; set; }
    }
}
