using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Employees;

public class EmployeesClient : BaseClient
{
    public EmployeesClient(HttpClient Client, string Address /* специально оставлена ошибка */) : base(Client, "api/employees")
    {
    }
}
