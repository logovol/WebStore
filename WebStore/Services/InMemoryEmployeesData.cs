using WebStore.Data;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Services;
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

        if(_Employees.Count > 0)
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

        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()
        return employee.Id;
    }

    public bool Delete(int Id)
    {
        var employee = GetById(Id);
        if (employee is null)
            return false;

        _Employees.Remove(employee);
        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()
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
            return false;

        db_employee.Id = employee.Id;
        db_employee.Name = employee.Name;
        db_employee.LastName = employee.LastName;
        db_employee.Patronymic = employee.Patronymic;
        db_employee.Age = employee.Age;
        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()

        return true;
    }

    public Employee? GetById(int id) 
    {
        var employee = _Employees.FirstOrDefault(e => e.Id == id);
        return employee;

    }
}
