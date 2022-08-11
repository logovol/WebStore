using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.Services;
using WebStore.Services.Data;
using WebStore.Services.Services.InCookies;
using WebStore.Services.Services.InSQL;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

// можно написать так (DB секция-раздел из appsettings.json, Type - ключ внутри секции)
//var db_type = config.GetSection("DB")["Type"];
var db_type = config["DB:Type"];
var db_connection_string = config.GetConnectionString(db_type);

switch (db_type)
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
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 3;
    opt.Password.RequiredUniqueChars = 3;
#endif

    opt.User.RequireUniqueEmail = false;
    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // для всех вновь созданных пользователей учетные записи не заблокированны
    opt.Lockout.AllowedForNewUsers = false;
    opt.Lockout.MaxFailedAccessAttempts = 10;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

services.AddScoped<IEmployeesData, SqlEmployeesData>();
services.AddScoped<IProductData, SqlProductData>();
services.AddScoped<ICartService, InCookiesCartService>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
