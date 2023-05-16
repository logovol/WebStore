using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Entities.Identity;
namespace WebStore.Interfaces.Identity;

internal interface IRolesClient : IRoleStore<Role>
{
}