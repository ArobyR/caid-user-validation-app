using Microsoft.AspNetCore.Identity;

namespace UserAuth.Models;
public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; }
}