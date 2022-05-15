var builder = WebApplication.CreateBuilder(args);

// конфигурирование основных частей (сервисов)
var app = builder.Build();

// из файла конфигурации appsettings.json
//var greetings = app.Configuration["ServerGreetings"];
//app.MapGet("/", () => "Hello World!");
//таким образом можно изменить информацию динамически
app.MapGet("/", () => app.Configuration["ServerGreetings"]);

app.Run();
