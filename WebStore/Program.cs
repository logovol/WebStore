var builder = WebApplication.CreateBuilder(args);

// конфигурирование основных частей (сервисов)

// настройка контроллеров с представлениями
builder.Services.AddControllersWithViews();

var app = builder.Build();

// из файла конфигурации appsettings.json
// var greetings = app.Configuration["ServerGreetings"];
// app.MapGet("/", () => "Hello World!");
// таким образом можно изменить информацию динамически
app.MapGet("/greetings", () => app.Configuration["ServerGreetings"]);

// маршрут автоматически
// app.MapDefaultControllerRoute();

// создание маршрутка с настройками
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
