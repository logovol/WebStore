using Microsoft.EntityFrameworkCore;

using WebStore.DAL.Context;
using WebStore.Data;
using WebStore.Infrastructure.Conventions;
using WebStore.Infrastructure.Middleware;
using WebStore.Services;
using WebStore.Services.InSQL;
using WebStore.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Добавление сервиса в конейтер. Указывается интерфейс и класс, который его реализует
//builder.Services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();  // объект создается единажды
services.AddScoped<IEmployeesData, InMemoryEmployeesData>();     // самый универсальный. единажды, но внутри контекста (внутри области, которую можно создать как-то)
//services.AddScoped<IProductData, InMemoryProductData>();
services.AddScoped<IProductData, SqlProductData>();

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
