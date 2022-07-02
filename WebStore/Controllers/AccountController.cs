using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.Entities.Identity;
using WebStore.ViewModels.Identity;

namespace WebStore.Controllers;

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

    // Отправка данных
    public IActionResult Register() => View(new RegisterUserViewModel());

    // Обработка полученных данных
    [HttpPost]
    [ValidateAntiForgeryToken] // Немного непонятно, то исключает снифферов и т.д.
    public async Task<IActionResult> Register(RegisterUserViewModel Model)
    {
        if (!ModelState.IsValid)
            return View(Model);

        var user = new User
        {
            UserName = Model.UserName,
        };

        var creation_result = await _UserManager.CreateAsync(user, Model.Password);
        if(creation_result.Succeeded)
        {
            _Logger.LogInformation("Пользователь {0} зарегистрирован", user);
            // Вход в систему. false - на один сеанс
            await _SignInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

        foreach (var error in creation_result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        var error_info = string.Join(", ", creation_result.Errors.Select(e => e.Description));
        _Logger.LogWarning("Ошибка при регистрации пользователя {0}:{1}", user, error_info);

        return View(Model);
    }

    // Login() отправляет ViewModel в Login.cshtml
    // ReturnUrl можно не указывать
    public IActionResult Login(string? ReturnUrl) => View(new LoginViewModel() { ReturnUrl = ReturnUrl });

    // Данный метод ловит форму обратно
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel Model)
    {
        if (!ModelState.IsValid)
            return View(Model);

        var login_result = await _SignInManager.PasswordSignInAsync(
            Model.UserName,
            Model.Password,
            Model.RememberMe,
            lockoutOnFailure: true);

        if(login_result.Succeeded)
        {
            _Logger.LogInformation("Пользователь {0} успешно вошёл в систему", Model.UserName);

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

        _Logger.LogWarning("Ошибка входа пользователя {0} - неверное имя или пароль", Model.UserName);

        return View(Model);
    }

    public async Task<IActionResult> Logout()
    {
        var user_name = User.Identity!.Name;

        await _SignInManager.SignOutAsync();

        _Logger.LogInformation("Пользователь {0} вышел из системы", user_name);
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();
}
