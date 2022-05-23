﻿using Microsoft.AspNetCore.Mvc;

using WebStore.Models;

namespace WebStore.Controllers
{
    //[Route("Staff/{action=Index}/{Id?}")]
    public class EmployeesController : Controller
    {
        private static readonly List<Employee> __Employees = new()
        {
            new Employee { Id = 1, LastName = "Иванов", Name = "Иван", Patronymic = "Иванович", Age = 23 },
            new Employee { Id = 2, LastName = "Петров", Name = "Пётр", Patronymic = "Петрович", Age = 27 },
            new Employee { Id = 3, LastName = "Сидоров", Name = "Сидор", Patronymic = "Сидорович", Age = 18 },
        };

        public IActionResult Index()
        {
            return View(__Employees);
        }

        //[Route("Staff/Info/{Id}")]
        //[Route("Info/{Id}")]        
        //[Route("[controller]/Info/{Id}")]
        public IActionResult Details(int Id)
        {
            var employee = __Employees.FirstOrDefault(x => x.Id == Id);
            if (employee is null)
                return NotFound();

            return View(employee);
        }
    }
}
