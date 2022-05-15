var builder = WebApplication.CreateBuilder(args);

// ���������������� �������� ������ (��������)
var app = builder.Build();

// �� ����� ������������ appsettings.json
//var greetings = app.Configuration["ServerGreetings"];
//app.MapGet("/", () => "Hello World!");
//����� ������� ����� �������� ���������� �����������
app.MapGet("/", () => app.Configuration["ServerGreetings"]);

app.Run();
