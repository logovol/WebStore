using Microsoft.Extensions.Logging;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;

namespace WebStore.Services.InSQL;

public class SqlEmployeesData : IEmployeesData
{
    private readonly WebStoreDB _db;
    private readonly ILogger<SqlEmployeesData> _Logger;

    public SqlEmployeesData(WebStoreDB db, ILogger<SqlEmployeesData> Logger)
    {
        _db = db;
        _Logger = Logger;
    }

    public int Add(Employee employee)
    {
        if (employee is null)
            throw new ArgumentNullException(nameof(employee));
        
        _db.Employees.Add(employee);

        _db.SaveChanges();

        _Logger.LogInformation("Сотрудник {0} добавлен", employee);

        // если работаем с БД, то нужно обязательно тут вызвать SaveChanges()
        return employee.Id;
    }

    public bool Delete(int Id)
    {
        //var employee = GetById(Id);
        
        // без загрузки лишней информации
        var employee = _db.Employees            
            .Select(e => new Employee { Id = e.Id })
            .FirstOrDefault(e => e.Id == Id);
        if (employee is null)
        {
            // при записи в журнал не нужно использовать !!!***### $ ###***!!!
            _Logger.LogWarning("При попытке удаления сотрудника с id:{0} - запись не найдена", Id);
            return false;
        }

        _db.Remove(employee);
        _db.SaveChanges();

        return true;
    }

    public bool Edit(Employee employee)
    {
        if (employee is null)
            throw new ArgumentNullException(nameof(employee));             

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

        _db.SaveChanges();

        _Logger.LogInformation("Сотрудник {0} отредактирован", employee);

        return true;
    }

    public IEnumerable<Employee> Get(int Skip, int Take)
    {
        if (Take == 0)
            return Enumerable.Empty<Employee>();

        IQueryable<Employee> query = _db.Employees;

        if(Skip > 0)
            query = query.Skip(Skip);

        return query.Take(Take);
    }

    public IEnumerable<Employee> GetAll() => _db.Employees;

    //public Employee? GetById(int id) => _db.Employees.FirstOrDefault(e => e.Id == id);
    // если будет два одинаковых, то выкинет исключение.
    //public Employee? GetById(int id) => _db.Employees.SingleOrDefault(e => e.Id == id);
    // оптимизация в том, что метод сначала ищет в кэше и если нет, то делает запрос в БД как FirstOfDefault
    public Employee? GetById(int id) => _db.Employees.Find(id);

    public int GetCount() => _db.Employees.Count();
}
