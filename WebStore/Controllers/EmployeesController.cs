﻿using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Infrastructure.Mapping;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers;

//[Route("Staff/{action=Index}/{Id?}")]
// ограничение доступа всем кто не авторизовался
[Authorize]
public class EmployeesController : Controller
{
    private readonly IEmployeesData _Employees;
    private readonly IMapper _Mapper;

    public EmployeesController(IEmployeesData Employees, IMapper Mapper)
    {
        _Employees = Employees;
        _Mapper = Mapper;
    }

    public IActionResult Index(int? Page, int PageSize = 15)
    {
        IEnumerable<Employee> employees;

        if (Page is { } page && PageSize > 0)
        {
            employees = _Employees.Get(page * PageSize, PageSize);
        }
        else
            employees = _Employees.GetAll();

        ViewBag.PagesCount = PageSize > 0 ?
            (int?)Math.Ceiling(_Employees.GetCount() / (double)PageSize)
            : null!; 

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

    [Authorize(Roles = Role.Administrators)]
    public IActionResult Create() => View("Edit", new EmployeeViewModel());

    [Authorize(Roles = Role.Administrators)]
    public IActionResult Edit(int? Id)
    {
        if (Id is null)
        {
            return View(new EmployeeViewModel());    
        }

        var employee = _Employees.GetById((int)Id);
        if (employee is null)
            return NotFound();

        var view_model = _Mapper.Map<EmployeeViewModel>(employee);


        //var view_model = employee.ToView();
        /*var view_model = new EmployeeViewModel()
        {
            Id = employee.Id,
            Name = employee.Name,
            LastName = employee.LastName,
            Patronymic = employee.Patronymic,
            Age = employee.Age,
        };*/

        return View(view_model);
    }

    [HttpPost]
    [Authorize(Roles = Role.Administrators)]
    public IActionResult Edit(EmployeeViewModel Model)
    {
        if (Model.LastName == "Qwe" && Model.Name == "Qwe" && Model.Patronymic == "Qwe")
            ModelState.AddModelError("", "Qwe - плохой выбор");

        if (Model.Name == "Asd")
            ModelState.AddModelError("Name", "Asd - плохой выбор"); // именно к этому свойству ошибка

        if (!ModelState.IsValid)
            return View(Model);

        var employee = _Mapper.Map<Employee>(Model);
        //var employee = Model.FromView();
        //var employee = new Employee()
        //{
        //    Id = Model.Id,
        //    Name = Model.Name,
        //    LastName = Model.LastName,
        //    Patronymic = Model.Patronymic!,
        //    Age = Model.Age,
        //};

        if (Model.Id == 0)
        {
            var new_employee_id = _Employees.Add(employee);
            return RedirectToAction(nameof(Details), new { Id = new_employee_id });
        }
        _Employees.Edit(employee);
        return RedirectToAction(nameof(Index));                
    }

    [Authorize(Roles = Role.Administrators)]
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

    [Authorize(Roles = Role.Administrators)]
    public IActionResult DeleteConfirmed(int Id)
    {
        if (!_Employees.Delete(Id))
            return NotFound();

        return RedirectToAction(nameof(Index));
    }
}
