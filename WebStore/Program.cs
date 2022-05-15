var builder = WebApplication.CreateBuilder(args);

// ���������������� �������� ������ (��������)

// ��������� ������������ � ���������������
builder.Services.AddControllersWithViews();

var app = builder.Build();

// �� ����� ������������ appsettings.json
// var greetings = app.Configuration["ServerGreetings"];
// app.MapGet("/", () => "Hello World!");
// ����� ������� ����� �������� ���������� �����������
app.MapGet("/greetings", () => app.Configuration["ServerGreetings"]);

// ������� �������������
// app.MapDefaultControllerRoute();

// �������� ��������� � �����������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
