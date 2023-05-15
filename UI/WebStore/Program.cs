using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Infrastructure;
using WebStore.Infrastructure.Conventions;
using WebStore.Interfaces.Services;
using WebStore.Interfaces.TestAPI;
using WebStore.Services.Data;
using WebStore.Services.Services.InCookies;
using WebStore.Services.Services.InSQL;
using WebStore.WebAPI.Clients.Employees;
using WebStore.WebAPI.Clients.Orders;
using WebStore.WebAPI.Clients.Products;
using WebStore.WebAPI.Clients.Values;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

// можно написать так (DB секция-раздел из appsettings.json, Type - ключ внутри секции)
//var db_type = config.GetSection("DB")["Type"];
var db_type = config["DB:Type"];
var db_connection_string = config.GetConnectionString(db_type);

switch(db_type)
{
    case "DockerDB":
    case "SqlServer":
        services.AddDbContext<WebStoreDB>(opt => opt.UseSqlServer(db_connection_string));
        break;
    case "Sqlite":
        services.AddDbContext<WebStoreDB>(opt => opt.UseSqlite(db_connection_string, o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
        break;
}

services.AddScoped<DbInitializer>();

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

// добавляем сервис как http-клиент и конфигурируем для него клиента (указываем базовый адрес в файле конфигурации)
services.AddHttpClient<IValuesService, ValuesClient>(client => client.BaseAddress = new(config["WebAPI"]));
services.AddHttpClient<IEmployeesData, EmployeesClient>(client => client.BaseAddress = new(config["WebAPI"]));
services.AddHttpClient<IProductData,   ProductsClient>(client => client.BaseAddress = new(config["WebAPI"]));
services.AddHttpClient<IOrderService,  OrdersClient>(client => client.BaseAddress = new(config["WebAPI"]));

// Добавление сервиса в конейтер. Указывается интерфейс и класс, который его реализует
//builder.Services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();  // объект создается единажды
//services.AddScoped<IEmployeesData, InMemoryEmployeesData>();     // самый универсальный. единажды, но внутри контекста (внутри области, которую можно создать как-то)
//services.AddScoped<IProductData, InMemoryProductData>();
//services.AddScoped<IEmployeesData, SqlEmployeesData>();
//services.AddScoped<IEmployeesData, EmployeesClient>();
//services.AddScoped<IProductData,   SqlProductData>();
//services.AddScoped<IOrderService,  SqlOrderService>();
services.AddScoped<ICartService,   InCookiesCartService>();
//services.AddScoped<IValuesService, ValuesClient>();

//builder.Services.AddTransient<IEmployeesData, InMemoryEmployeesData>();  // при каждом заспросе объект создается заново
// конфигурирование основных частей (сервисов)

// настройка контроллеров с представлениями
builder.Services.AddControllersWithViews(opt =>
    {
        // добавление нашего соглашения в нашу модель
        opt.Conventions.Add(new TestConvention());        
        opt.Conventions.Add(new AddAreaToControllerConvention());
    });

services.AddAutoMapper(typeof(Program));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{ 
    var db_initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await db_initializer.InitializeAsync(
        RemoveBefore: app.Configuration.GetValue("DB:Recreate", false),
        AddTestData: app.Configuration.GetValue("DB:AddTestData", false));
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

// создание маршрутов с настройками
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
