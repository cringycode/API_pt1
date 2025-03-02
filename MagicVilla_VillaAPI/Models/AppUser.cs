using Microsoft.AspNetCore.Identity;

namespace MagicVilla_VillaAPI.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
}