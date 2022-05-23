using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebStore.Models;

namespace WebStore.Data
{
    public static class TestData
    {
        public static ICollection<Employee> Employees { get; } = new List<Employee>
        {
            new() { Id = 1, LastName = "Иванов", Name = "Иван", Patronymic = "Иванович", Age = 23 },
            new() { Id = 2, LastName = "Петров", Name = "Пётр", Patronymic = "Петрович", Age = 27 },
            new() { Id = 3, LastName = "Сидоров", Name = "Сидор", Patronymic = "Сидорович", Age = 18 },
        };
    }
}
