using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WebStore.DAL.Context;
using WebStore.Data;
using WebStore.Domain.Entities.Identity;
using WebStore.Infrastructure.Conventions;
using WebStore.Infrastructure.Middleware;
using WebStore.Services;
using WebStore.Services.InCookies;
using WebStore.Services.InSQL;
using WebStore.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// конфигурирование системы Identity может быть тут /*opt => { opt... }*/
services.AddIdentity<User, Role>(/*opt => { opt... }*/)
    .AddEntityFrameworkStores<WebStoreDB>()
    .AddDefaultTokenProviders();

// Настройки Identity
services.Configure<IdentityOptions>(opt =>
{
#if DEBUG
    opt.Password.RequireDigit           = false;
    opt.Password.RequireLowercase       = false;
    opt.Password.RequireUppercase       = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength         = 3;
    opt.Password.RequiredUniqueChars    = 3;
#endif

    opt.User.RequireUniqueEmail         = false;
    opt.User.AllowedUserNameCharacters  = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // для всех вновь созданных пользователей учетные записи не заблокированны
    opt.Lockout.AllowedForNewUsers      = false;
    opt.Lockout.MaxFailedAccessAttempts = 10;
    opt.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
});

// Настраиваем cookies
services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.Name = "GB.WebStore";
    opt.Cookie.HttpOnly = true;

    opt.ExpireTimeSpan    = TimeSpan.FromDays(10);
    opt.LoginPath         = "/Account/Login";
    opt.LogoutPath        = "/Account/Logout";
    opt.AccessDeniedPath  = "/Account/AccessDenied";

    // Сброс идентификатора сеанса при входе и выходе (для безопасности)
    opt.SlidingExpiration = true;
});


// Добавление сервиса в конейтер. Указывается интерфейс и класс, который его реализует
//builder.Services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();  // объект создается единажды
//services.AddScoped<IEmployeesData, InMemoryEmployeesData>();     // самый универсальный. единажды, но внутри контекста (внутри области, которую можно создать как-то)
//services.AddScoped<IProductData, InMemoryProductData>();
services.AddScoped<IEmployeesData, SqlEmployeesData>();

services.AddScoped<IProductData, SqlProductData>();
services.AddScoped<ICartService, InCookiesCartService>();

services.AddDbContext<WebStoreDB>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
services.AddScoped<DbInitializer>();

//builder.Services.AddTransient<IEmployeesData, InMemoryEmployeesData>();  // при каждом заспросе объект создается заново
// конфигурирование основных частей (сервисов)

// настройка контроллеров с представлениями
builder.Services.AddControllersWithViews(opt =>
    {
        // добавление нашего соглашения в нашу модель
        opt.Conventions.Add(new TestConvention());
    });

services.AddAutoMapper(typeof(Program));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{ 
    var db_initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await db_initializer.InitializeAsync(
        RemoveBefore: app.Configuration.GetValue("DbRecreate", false),
        AddTestData: app.Configuration.GetValue("DbAddTestData", false));
}

// подключение страницы отладчика, не будет работать, когда проект будет на хостинге
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TestMiddleware>();

// из файла конфигурации appsettings.json
// var greetings = app.Configuration["ServerGreetings"];
// app.MapGet("/", () => "Hello World!");
// таким образом можно изменить информацию динамически
app.MapGet("/greetings", () => app.Configuration["ServerGreetings"]);

// промежуточное ПО. Перехватывает страницу главного маршрута и выводит страницу приветствия
app.UseWelcomePage("/welcome");

// маршрут автоматически
// app.MapDefaultControllerRoute();

// создание маршрута с настройками
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
