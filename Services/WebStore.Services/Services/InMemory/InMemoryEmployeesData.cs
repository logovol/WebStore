﻿using Microsoft.Extensions.Logging;

using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.Services.Data;

namespace WebStore.Services.Services.InMemory;
// СЕРВИС
public class InMemoryEmployeesData : IEmployeesData
{
    private readonly ILogger<InMemoryEmployeesData> _Logger;
    private readonly ICollection<Employee> _Employees;
    private int _LastFreeId;

    public InMemoryEmployeesData(ILogger<InMemoryEmployeesData> Logger)
    {
        _Employees = TestData.Employees;
        _Logger = Logger;

        if (_Employees.Count > 0)
            _LastFreeId = _Employees.Max(e => e.Id) + 1;
        else
            _LastFreeId = 1;
    }


    public IEnumerable<Employee> GetAll()
    {
        return _Employees;
    }

    public int Add(Employee employee)
    {
        if (employee is null)
            throw new ArgumentNullException(nameof(employee));

        // это нужно делать только для хранения данных в памяти
        // для БД не использовать
        if (_Employees.Contains(employee))
        {
            return employee.Id;
        }

        employee.Id = _LastFreeId;  // тоже не для БД
        _LastFreeId++;              // тоже не для БД

        _Employees.Add(employee);

        _Logger.LogInformation("Сотрудник {0} добавлен", employee);

        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()
        return employee.Id;
    }

    public bool Delete(int Id)
    {
        var employee = GetById(Id);
        if (employee is null)
        {
            // при записи в журнал не нужно использовать !!!***### $ ###***!!!
            _Logger.LogWarning("При попытке удаления сотрудника с id:{0} - запись не найдена", Id);
            return false;
        }

        _Employees.Remove(employee);
        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()

        _Logger.LogInformation("Сотрудник {0} удалён", employee);

        return true;
    }

    public bool Edit(Employee employee)
    {
        if (employee is null)
            throw new ArgumentNullException(nameof(employee));

        // это нужно делать только для хранения данных в памяти
        // для БД не использовать
        if (_Employees.Contains(employee))
        {
            return true;
        }

        var db_employee = GetById(employee.Id);
        if (db_employee is null)
        {
            // при записи в журнал не нужно использовать !!!***### $ ###***!!!
            _Logger.LogWarning("При попытке редактирования сотрудника с id:{0} - запись не найдена", employee);
            return false;
        }

        db_employee.Id = employee.Id;
        db_employee.Name = employee.Name;
        db_employee.LastName = employee.LastName;
        db_employee.Patronymic = employee.Patronymic;
        db_employee.Age = employee.Age;
        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()

        _Logger.LogInformation("Сотрудник {0} отредактирован", employee);

        return true;
    }

    public Employee? GetById(int id)
    {
        var employee = _Employees.FirstOrDefault(e => e.Id == id);

        return employee;
    }

    public int GetCount() => _Employees.Count;

    public IEnumerable<Employee> Get(int Skip, int Take)// => _Employees.Skip(Skip).Take(Take);
    {
        IEnumerable<Employee> query = _Employees;

        if (Take == 0)
            return Enumerable.Empty<Employee>();

        if (Skip > 0)
            query = query.Skip(Skip);

        if (Skip > _Employees.Count())
            return Enumerable.Empty<Employee>();

        return query.Take(Skip);
    }

}
