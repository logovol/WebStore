using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebStore.Domain.Entities;

namespace WebStore.Services.Interfaces
{
    // CRUD interface
    public interface IEmployeesData
    {
        int GetCount();

        IEnumerable<Employee> GetAll();

        IEnumerable<Employee> Get(int Skip, int Take);

        Employee? GetById(int id);

        int Add(Employee employee);

        bool Edit(Employee employee);
        
        bool Delete(int Id);
    }
}
