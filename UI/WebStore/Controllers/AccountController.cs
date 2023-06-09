﻿using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels.Identity;

namespace WebStore.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<User> _UserManager;
    private readonly SignInManager<User> _SignInManager;
    private readonly ILogger<AccountController> _Logger;

    public AccountController(
        UserManager<User> UserManager,
        SignInManager<User> SignInManager,
        ILogger<AccountController> Logger)
    {
        _UserManager = UserManager;
        _SignInManager = SignInManager;
        _Logger = Logger;
    }

    // Разрешаем доступ
    [AllowAnonymous]
    // Отправка данных
    public IActionResult Register() => View(new RegisterUserViewModel());

    // Обработка полученных данных
    [HttpPost]
    [AllowAnonymous] // разрешаем доступ
    [ValidateAntiForgeryToken] // Немного непонятно, то исключает снифферов и т.д.
    public async Task<IActionResult> Register(RegisterUserViewModel Model)
    {
        if (!ModelState.IsValid)
            return View(Model);

        var user = new User
        {
            UserName = Model.UserName,
        };

        _Logger.LogTrace("Начало процесса регистрации нового пользователя {0)", Model.UserName);
        var timer = Stopwatch.StartNew();

        using var logger_scope = _Logger.BeginScope("Регистрация нового пользователя {0}", Model.UserName);

        var creation_result = await _UserManager.CreateAsync(user, Model.Password);

        if (creation_result.Succeeded)
        {
            _Logger.LogInformation("Пользователь {0} зарегистрирован за {1} мс", user, timer.ElapsedMilliseconds);

            await _UserManager.AddToRoleAsync(user, Role.Users);

            _Logger.LogInformation("Пользователю {0} назначена роль {1}. {2} мс", user, Role.Users, timer.ElapsedMilliseconds);

            // Вход в систему. false - на один сеанс
            await _SignInManager.SignInAsync(user, false);
            _Logger.LogTrace("Пользователю {0} вошёл в систему. {1} мс", user, timer.ElapsedMilliseconds);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in creation_result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        var error_info = string.Join(", ", creation_result.Errors.Select(e => e.Description));
        _Logger.LogWarning("Ошибка при регистрации пользователя {0}:({1} мс):{2}", user, timer.ElapsedMilliseconds, error_info);

        return View(Model);
    }

    // Login() отправляет ViewModel в Login.cshtml
    // ReturnUrl можно не указывать
    [AllowAnonymous]
    public IActionResult Login(string? ReturnUrl) => View(new LoginViewModel() { ReturnUrl = ReturnUrl });

    // Данный метод ловит форму обратно
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel Model)
    {
        if (!ModelState.IsValid)
            return View(Model);

        _Logger.LogTrace("Начат вход в систему пользователя {0}", Model.UserName);

        using var logger_scope = _Logger.BeginScope("Вход в систему пользователя {0}", Model.UserName);

        var timer = Stopwatch.StartNew();

        var login_result = await _SignInManager.PasswordSignInAsync(
            Model.UserName,
            Model.Password,
            Model.RememberMe,
            lockoutOnFailure: true);

        if (login_result.Succeeded)
        {
            _Logger.LogInformation("Пользователь {0} успешно вошёл в систему. {1} мс", Model.UserName, timer.ElapsedMilliseconds);

            // делать редирект просто так нельзя. мб подмена на другой сайт и cookies уйдет злоумышленнику
            //return Redirect(Model.ReturnUrl);

            // Так нужно делать
            //if(Url.IsLocalUrl(Model.ReturnUrl))
            //    return Redirect(Model.ReturnUrl);
            //return RedirectToAction("Index", "Home");

            // Можно написать проще
            return LocalRedirect(Model.ReturnUrl ?? "/");
        }

        // Увеомление поьзователя, если он не вошел в систему
        ModelState.AddModelError("", "Неверное имя пользователя или пароль");

        _Logger.LogWarning("Ошибка входа пользователя {0} - неверное имя или пароль. {1} мс.", Model.UserName, timer.ElapsedMilliseconds);

        return View(Model);
    }

    public async Task<IActionResult> Logout()
    {
        var user_name = User.Identity!.Name;

        await _SignInManager.SignOutAsync();

        _Logger.LogInformation("Пользователь {0} вышел из системы", user_name);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied(string? ReturnUrl)
    {
        ViewBag.ReturnUrl = ReturnUrl;
        return View();
    }
}
