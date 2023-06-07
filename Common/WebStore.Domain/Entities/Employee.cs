using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities.Base;

namespace WebStore.Domain.Entities;

/// <summary>Сотрудник</summary>
[Index(nameof(Name), nameof(LastName), nameof(Patronymic), nameof(Age), IsUnique = true)]
public class Employee : Entity
{
    /// <summary>Имя</summary>
    [Required]
    public string Name { get; set; } = null!;
    /// <summary>Фамилия</summary>
    [Required]
    public string LastName { get; set; } = null!;
    /// <summary>Отчество</summary>
    public string? Patronymic { get; set; } = null!;
    /// <summary>Возраст</summary>
    public int Age { get; set; }

    public override string ToString() => $"(id:{Id}){LastName}{Name}{Patronymic} - age:{Age}";
}
