using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Entities.Identity;
namespace WebStore.Interfaces.Identity;

public interface IRolesClient : IRoleStore<Role>
{
}