using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace WebStore.Domain.ViewModels;

public class EmployeeViewModel : IValidatableObject
{
    [HiddenInput(DisplayValue = false)]
    [Display(Name = "Идентификатор")]
    public int Id { get; set; }
    [Display(Name = "Имя")]                      // атрибут визуализации
    [Required(ErrorMessage = "Имя обязательно")] // атрибут валидации
    [StringLength(10, MinimumLength = 2, ErrorMessage = "Длина строки от 2 до 10 символов")]
    [RegularExpression("([А-ЯЁ][а-яё]+)|([A-Z][a-z]+)", ErrorMessage = "Неверерный формат имени. Либо все русские, либо все латинские. Первая заглавная")]
    public string Name { get; set; } = null!;
    [Display(Name = "Фамилия")]
    [Required(ErrorMessage = "Фамилия обязательная")]
    [StringLength(10, MinimumLength = 2, ErrorMessage = "Длина строки от 2 до 10 символов")]
    public string LastName { get; set; } = null!;
    [Display(Name = "Отчество")]
    [StringLength(10, ErrorMessage = "Длина строки до 10 символов")]
    public string? Patronymic { get; set; } = null!;
    [Display(Name = "Возраст")]
    [Range(18, 80, ErrorMessage = "Возраст должен быть в диапозоне от 18 до 80 лет")]
    public int Age { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LastName == "Asd" && Name == "Asd" && Patronymic == "Asd")
            return new[]
            {
                new ValidationResult("Везде Asd", new[]
                {
                    nameof(Name),
                    nameof(LastName),
                    nameof(Patronymic),
                })
            };

        return new[]
        {
            ValidationResult.Success!,
        };

    }
}
