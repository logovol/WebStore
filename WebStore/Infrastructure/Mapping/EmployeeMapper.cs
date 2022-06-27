using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Models;
using WebStore.ViewModels;

namespace WebStore.Infrastructure.Mapping
{
    public static class EmployeeMapper
    {
        [return: NotNullIfNotNull("employee")]
        public static EmployeeViewModel? ToView(this Employee? employee) => employee is null
            ? null
            : new EmployeeViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                LastName = employee.LastName,
                Patronymic = employee.Patronymic,
                Age = employee.Age,
            };

        [return:NotNullIfNotNull("employee")]
        public static Employee? FromView(this EmployeeViewModel? employee) => employee is null
            ? null
            : new Employee
            {
                Id = employee.Id,
                Name = employee.Name,
                LastName = employee.LastName,
                Patronymic = employee.Patronymic!,
                Age = employee.Age,
            };

        public static IEnumerable<EmployeeViewModel?> ToView(this IEnumerable<Employee?> employees) => employees.Select(ToView);
        public static IEnumerable<Employee?> FromView(this IEnumerable<EmployeeViewModel?> employees) => employees.Select(FromView);
    }
}
