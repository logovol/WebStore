using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

using Polly;
using Polly.Extensions.Http;

using Serilog;
using Serilog.Formatting.Json;

using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Hubs;
using WebStore.Infrastructure;
using WebStore.Infrastructure.Conventions;
using WebStore.Interfaces.Services;
using WebStore.Interfaces.Services.Identity;
using WebStore.Interfaces.TestAPI;
using WebStore.Logging;
using WebStore.Services.Data;
using WebStore.Services.Services;
using WebStore.Services.Services.InCookies;
using WebStore.WebAPI.Clients.Employees;
using WebStore.WebAPI.Clients.Identity;
using WebStore.WebAPI.Clients.Orders;
using WebStore.WebAPI.Clients.Products;
using WebStore.WebAPI.Clients.Values;


var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddLog4Net();

//builder.Host.ConfigureLogging(
//    log => log
//        .ClearProviders()
//        .AddConsole()
//        .AddEventLog(opt => opt.LogName = "WebStore-log")
//        .AddDebug()
//        .AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Warning));


builder.Host.UseSerilog((host, log) => log.ReadFrom.Configuration(host.Configuration)
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}]{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
    .WriteTo.RollingFile(@"UI\Logs\WebStore[{DateTime.Now:yyyy-MM-ddTHH-mm-ss}].log")
    .WriteTo.File(new JsonFormatter(",\r\n", true), @"\Logs\WebStore[{DateTime.Now:yyyy-MM-ddTHH-mm-ss}].log.json")
    .WriteTo.Seq(host.Configuration["SeqAddress"]!)
    );

var config = builder.Configuration;
var services = builder.Services;

//services.Configure<ConsoleFormatterOptions>(opt => opt.IncludeScopes = true);

//// можно написать так (DB секция-раздел из appsettings.json, Type - ключ внутри секции)
////var db_type = config.GetSection("DB")["Type"];
//var db_type = config["DB:Type"];
//var db_connection_string = config.GetConnectionString(db_type);

//switch(db_type)
//{
//    case "DockerDB":
//    case "SqlServer":
//        services.AddDbContext<WebStoreDB>(opt => opt.UseSqlServer(db_connection_string));
//        break;
//    case "Sqlite":
//        services.AddDbContext<WebStoreDB>(opt => opt.UseSqlite(db_connection_string, o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
//        break;
//}

//services.AddScoped<DbInitializer>();

// конфигурирование системы Identity может быть тут /*opt => { opt... }*/
services.AddIdentity<User, Role>(/*opt => { opt... }*/)
    //.AddEntityFrameworkStores<WebStoreDB>()
    .AddDefaultTokenProviders();

services.AddHttpClient("WebStoreAPIIdentity", client =>
    {
        //client.DefaultRequestHeaders.Add("accept", "application/json");
        client.BaseAddress = new(config["WebAPI"]);
    })
    .AddTypedClient<IUsersClient, UsersClient>()
    .AddTypedClient<IUserStore<User>,               UsersClient>()
    .AddTypedClient<IUserRoleStore<User>,           UsersClient>()
    .AddTypedClient<IUserPasswordStore<User>,       UsersClient>()
    .AddTypedClient<IUserEmailStore<User>,          UsersClient>()
    .AddTypedClient<IUserPhoneNumberStore<User>,    UsersClient>()
    .AddTypedClient<IUserTwoFactorStore<User>,      UsersClient>()
    .AddTypedClient<IUserClaimStore<User>,          UsersClient>()
    .AddTypedClient<IUserLoginStore<User>,          UsersClient>()
    .AddTypedClient<IRolesClient,                   RolesClient>()
    .AddTypedClient<IRoleStore<Role>,               RolesClient>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());


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

// Один раз конфигурируем клиент, а далее подключаем к нему сервисы (взамен закоментированного кода далее)
services.AddHttpClient("WebStoreApi", client => client.BaseAddress = new(config["WebAPI"]))
    .AddTypedClient<IValuesService, ValuesClient>()
    .AddTypedClient<IEmployeesData, EmployeesClient>()
    .AddTypedClient<IProductData, ProductsClient>()
    .AddTypedClient<IOrderService, OrdersClient>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// опеределяем политику повторных запросов
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int MaxRetryPolicy = 5, int MaxJitterTime = 1000)
{
    var jitter = new Random();
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(MaxRetryPolicy, RetryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, RetryAttempt)) +
            TimeSpan.FromMilliseconds(jitter.Next(0, MaxJitterTime)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 5, TimeSpan.FromSeconds(30));

// добавляем сервис как http-клиент и конфигурируем для него клиента (указываем базовый адрес в файле конфигурации)
//services.AddHttpClient<IValuesService, ValuesClient>(client => client.BaseAddress = new(config["WebAPI"]));
//services.AddHttpClient<IEmployeesData, EmployeesClient>(client => client.BaseAddress = new(config["WebAPI"]));
//services.AddHttpClient<IProductData,   ProductsClient>(client => client.BaseAddress = new(config["WebAPI"]));
//services.AddHttpClient<IOrderService,  OrdersClient>(client => client.BaseAddress = new(config["WebAPI"]));

// Добавление сервиса в конейтер. Указывается интерфейс и класс, который его реализует
//builder.Services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();  // объект создается единажды
//services.AddScoped<IEmployeesData, InMemoryEmployeesData>();     // самый универсальный. единажды, но внутри контекста (внутри области, которую можно создать как-то)
//services.AddScoped<IProductData, InMemoryProductData>();
//services.AddScoped<IEmployeesData, SqlEmployeesData>();
//services.AddScoped<IEmployeesData, EmployeesClient>();
//services.AddScoped<IProductData,   SqlProductData>();
//services.AddScoped<IOrderService,  SqlOrderService>();
services.AddScoped<ICartStore, InCookiesCartStore>();
services.AddScoped<ICartService, CartService>();
//services.AddScoped<ICartService, InCookiesCartService>();
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

services.AddSignalR();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{ 
//    var db_initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
//    await db_initializer.InitializeAsync(
//        RemoveBefore: app.Configuration.GetValue("DB:Recreate", false),
//        AddTestData: app.Configuration.GetValue("DB:AddTestData", false));
//}

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
    endpoints.MapHub<ChatHub>("/chat");

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
