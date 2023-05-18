using WebStore.Interfaces;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Identity;

public class UsersClient : BaseClient
{
    public UsersClient(HttpClient Client) : base(Client, WebApiAddresses.V1.Identity.Users)
    {

    }
}
