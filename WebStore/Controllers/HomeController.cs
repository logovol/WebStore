﻿using Microsoft.AspNetCore.Mvc;
using WebStore.Models;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        private static readonly List<Employee> __Employees = new()
        {
            new Employee { Id = 1, LastName = "Иванов", Name = "Иван", Patronymic = "Иванович", Age = 23 },
            new Employee { Id = 2, LastName = "Петров", Name = "Пётр", Patronymic = "Петрович", Age = 27 },
            new Employee { Id = 3, LastName = "Сидоров", Name = "Сидор", Patronymic = "Сидорович", Age = 18 },
        };

        public IActionResult Index()
        {
            return View();
            //return Content("Index action");
        }

        public IActionResult Greetings(string? id)
        {
            return Content($"Hello from first controller - {id}");
        }

        public IActionResult Employees()
        {
            return View(__Employees);
        }

        public IActionResult EmployeeDetails(int Id)
        {
            var employee = __Employees.FirstOrDefault(x => x.Id == Id);
            if (employee is null)
                return NotFound();
            
            return View(employee);
            
        }

        public IActionResult Contacts() => View();

        public IActionResult Error404() => View();
    }
}
