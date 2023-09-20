using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace WebStore.Domain.ViewModels.Identity;

public class RegisterUserViewModel
{
    [Required(ErrorMessage = "Имя пользователя не указано")]
    [Display(Name = "Имя пользователя")]
    [MaxLength(255)]
    [Remote("IsNameFree", "Account")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Пароль является обязательным")]
    [Display(Name = "Пароль")]
    [MaxLength(255)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Не введено подтверждение пароля")]
    [Display(Name = "Подтверждение пароля")]
    [DataType(DataType.Password)]
    [MaxLength(255)]
    [Compare(nameof(Password), ErrorMessage = "Пароль и подтверждение не совпадают")]
    public string PasswordConfirm { get; set; } = null!;
}
