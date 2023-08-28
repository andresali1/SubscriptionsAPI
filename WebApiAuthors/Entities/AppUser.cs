using Microsoft.AspNetCore.Identity;

namespace WebApiAuthors.Entities
{
    public class AppUser : IdentityUser
    {
        public bool BadPaymentHistory { get; set; }
    }
}
