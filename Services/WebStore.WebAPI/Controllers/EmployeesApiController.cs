﻿using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.Entities;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

/// <summary>Управление сотрудниками</summary>
[ApiController]
[Route(WebApiAddresses.V1.Employees)]
public class EmployeesApiController : ControllerBase
{
    private readonly IEmployeesData _EmployeesData;
    private readonly ILogger<EmployeesApiController> _Logger;

    public EmployeesApiController(IEmployeesData EmployeesData, ILogger<EmployeesApiController> Logger)
    {
        _EmployeesData = EmployeesData;
        _Logger = Logger;
    }
    /// <summary>Количество сотрудников</summary>
    /// <returns></returns>
    [HttpGet("count")] // GET -> api/employees/count
    public IActionResult GetCount()
    {
        var result = _EmployeesData.GetCount();
        return Ok(result);
    }
    
    /// <summary>Полный список сотрудников</summary>
    /// <returns></returns>
    [HttpGet] // GET -> api/employees/count
    public IActionResult GetAll()
    {
        if (_EmployeesData.GetCount() == 0)
        {
            return NoContent();
        }

        var result = _EmployeesData.GetAll();
        return Ok(result);
    }


    /// <summary>Фрагмент списка сотрудников</summary>
    /// <param name="Skip">Пропускаемое количество элементов в начале списка</param>
    /// <param name="Take">Количество элементов в выборке</param>
    /// <returns></returns>
    [HttpGet("[[{Skip:int}/{Take:int}]]")] // GET -> api/employees/[2:4] пропустить 2, получить 4 элемента
    [HttpGet("{Skip:int}/{Take:int}")]   // GET -> api/employees/2/4  пропустить 2, получить 4 элемента
    public IActionResult Get(int Skip, int Take)
    {
        if (Skip < 0 || Take < 0)
            return BadRequest();

        if (Take == 0 || Skip > _EmployeesData.GetCount())
            return NoContent();

        var result = _EmployeesData.Get(Skip, Take);
        return Ok(result);
    }

    /// <summary>Сотрудник с заданным идентификатором</summary>
    /// <param name="id">Идентификатор сотрудника</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var result = _EmployeesData.GetById(id);
        return result is null
            ? NotFound()
            :Ok(result);
    }
    
    /// <summary>Добавление нового сотрудника</summary>
    /// <param name="employee">Добавляемый сотрудник</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Add([FromBody] Employee employee)
    {
        var id = _EmployeesData.Add(employee);
        return CreatedAtAction(nameof(GetById), new { Id = id }, employee);
    }

    /// <summary>Внесение изменений в информацию о сотруднике</summary>
    /// <param name="employee">Структура с новой информацией о сотруднике</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult Edit([FromBody] Employee employee)
    {
        var result = _EmployeesData.Edit(employee);
        if(result)
            return Ok(true);
        return NotFound(false);
    }

    /// <summary>Удаление сотрудника</summary>
    /// <param name="Id">Идентификатор сотрудника</param>
    /// <returns></returns>
    [HttpDelete("{Id:int}")]
    public IActionResult Delete(int Id)
    {
        var result = _EmployeesData.Delete(Id);
        return result ? Ok() : NotFound();
    }
}
