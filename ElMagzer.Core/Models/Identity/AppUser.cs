
using Microsoft.AspNetCore.Identity;

namespace ElMagzer.Core.Models.Identity
{
    public class AppUser:IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
