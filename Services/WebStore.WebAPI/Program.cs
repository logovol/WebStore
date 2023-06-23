using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.Services;
using WebStore.Logging;
using WebStore.Services.Data;
using WebStore.Services.Services.InCookies;
using WebStore.Services.Services.InSQL;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddLog4Net();

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
services.AddScoped<IOrderService, SqlOrderService>();
// один из предложенных вариантов из StackOverflow
//services
//    .AddMvc()
//    .AddXmlSerializerFormatters();
services.AddControllers(opt =>
{
    // настройка системы WebAPI на передачу данных в нужном формате
    opt.InputFormatters.Add(new XmlSerializerInputFormatter(opt));
    opt.OutputFormatters.Add(new XmlSerializerOutputFormatter());

});
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(opt =>
{
    //const string webstore_webapi_xml = "WebStore.WebAPI.xml";
    //const string webstore_domain_xml = "WebStore.Domain.xml";

    var webstore_webapi_xml = Path.ChangeExtension(Path.GetFileName(typeof(Program).Assembly.Location), "xml.");
    var webstore_domain_xml = Path.ChangeExtension(Path.GetFileName(typeof(Product).Assembly.Location), "xml.");

    const string debug_path = "bin/Debug/net6.0";

    if (File.Exists(webstore_webapi_xml))
        opt.IncludeXmlComments(webstore_webapi_xml);
    else if (File.Exists(Path.Combine(debug_path, webstore_webapi_xml)))
        opt.IncludeXmlComments(Path.Combine(debug_path, webstore_webapi_xml));

    if (File.Exists(webstore_domain_xml))
        opt.IncludeXmlComments(webstore_domain_xml);
    else if (File.Exists(Path.Combine(debug_path, webstore_domain_xml)))
        opt.IncludeXmlComments(Path.Combine(debug_path, webstore_domain_xml));    
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db_initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await db_initializer.InitializeAsync(
        RemoveBefore: app.Configuration.GetValue("DB:Recreate", false),
        AddTestData: app.Configuration.GetValue("DB:AddTestData", false));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
