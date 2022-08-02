using Microsoft.AspNetCore.Identity;

namespace WebStore.Domain.Entities.Identity;

public class User : IdentityUser
{
    public const string Administrator = "Admin";
    public const string AdminPassword = "AdWord#1_2_3";
    public override string ToString() => UserName;
}