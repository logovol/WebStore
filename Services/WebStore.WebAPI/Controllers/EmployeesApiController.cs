﻿using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesApiController : ControllerBase
{
    private readonly IEmployeesData _EmployeesData;
    private readonly ILogger<EmployeesApiController> _Logger;

    public EmployeesApiController(IEmployeesData EmployeesData, ILogger<EmployeesApiController> Logger)
    {
        _EmployeesData = EmployeesData;
        _Logger = Logger;
    }

    [HttpGet("count")] // GET -> api/employees/count
    public IActionResult GetCount()
    {
        var result = _EmployeesData.GetCount();
        return Ok(result);
    }

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

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var result = _EmployeesData.GetById(id);
        return result is null
            ? NotFound()
            :Ok(result);
    }
    
    [HttpPost]
    public IActionResult Add([FromBody] Employee employee)
    {
        var id = _EmployeesData.Add(employee);
        return CreatedAtAction(nameof(GetById), new { Id = id }, employee);
    }

    [HttpPut]
    public IActionResult Edit([FromBody] Employee employee)
    {
        var result = _EmployeesData.Edit(employee);
        if(result)
            return Ok(true);
        return NotFound(false);
    }

    [HttpDelete("{Id:int}")]
    public IActionResult Delete(int Id)
    {
        var result = _EmployeesData.Delete(Id);
        return result ? Ok() : NotFound();
    }
}
