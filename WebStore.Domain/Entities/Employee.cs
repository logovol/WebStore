namespace WebStore.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Patronymic { get; set; } = null!;
    public int Age { get; set; }

    public override string ToString() => $"(id:{Id}){LastName}{Name}{Patronymic} - age:{Age}";
}
