﻿using Microsoft.AspNetCore.Mvc;

using WebStore.Models;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers;

//[Route("Staff/{action=Index}/{Id?}")]
public class EmployeesController : Controller
{
    private readonly IEmployeesData _Employees;
    
    public EmployeesController(IEmployeesData Employees) => _Employees = Employees;

    public IActionResult Index()
    {
        var employees = _Employees.GetAll();
        return View(employees);
    }

    //[Route("Staff/Info/{Id}")]
    //[Route("Info/{Id}")]        
    //[Route("[controller]/Info/{Id}")]
    public IActionResult Details(int Id)
    {
        var employee = _Employees.GetById(Id);
        if (employee is null)
            return NotFound();

        return View(employee);
    }

    public IActionResult Create() => View("Edit", new EmployeeViewModel());

    public IActionResult Edit(int? Id)
    {
        if (Id is null)
        {
            return View(new EmployeeViewModel());    
        }

        var employee = _Employees.GetById((int)Id);
        if (employee is null)
            return NotFound();

        var view_model = new EmployeeViewModel()
        {
            Id = employee.Id,
            Name = employee.Name,
            LastName = employee.LastName,
            Patronymic = employee.Patronymic,
            Age = employee.Age,
        };

        return View(view_model);
    }

    [HttpPost]
    public IActionResult Edit(EmployeeViewModel Model)
    {
        var employee = new Employee()
        {
            Id = Model.Id,
            Name = Model.Name,
            LastName = Model.LastName,
            Patronymic = Model.Patronymic,
            Age = Model.Age,
       };

        if (Model.Id == 0)
        {
            var new_employee_id = _Employees.Add(employee);
            return RedirectToAction(nameof(Details), new { Id = new_employee_id });
        }

        _Employees.Edit(employee);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int Id)
    {
        // опасно, взломают и удалят всех сотрудников
        // операции удаления никогда не делаются с помощью GET запросов
        //_Employees.Delete(Id);                    // так не делать
        //return RedirectToAction(nameof(Index));   // так не делать

        var employee = _Employees.GetById(Id);
        if (employee is null)
            return NotFound();

        var view_model = new EmployeeViewModel()
        {
            Id = employee.Id,
            Name = employee.Name,
            LastName = employee.LastName,
            Patronymic = employee.Patronymic,
            Age = employee.Age,
        };

        return View(view_model);
    }

    public IActionResult DeleteConfirmed(int Id)
    {
        if (!_Employees.Delete(Id))
            return NotFound();

        return RedirectToAction(nameof(Index));
    }
}
