using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Services.Data;

// класс занимается инициализацией БД
public class DbInitializer
{
    private readonly WebStoreDB _db;
    private readonly UserManager<User> _UserManager;
    private readonly RoleManager<Role> _RoleManager;
    private readonly ILogger<DbInitializer> _Logger;

    public DbInitializer(
        WebStoreDB db,
        UserManager<User> UserManager,
        RoleManager<Role> RoleManager,
        ILogger<DbInitializer> Logger)
    {
        _db = db;
        _UserManager = UserManager;
        _RoleManager = RoleManager;
        _Logger = Logger;
    }

    public async Task<bool> RemoveAsync(CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Удаление БД...");
        var result = await _db.Database.EnsureDeletedAsync(Cancel).ConfigureAwait(false);
        if (result)
            _Logger.LogInformation("Удаление БД выполнено успешно");
        else
            _Logger.LogInformation("Удаление БД не выполнено или она отсутствовала на сервере");

        return result;
    }

    public async Task InitializeAsync(bool RemoveBefore, bool AddTestData, CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Инициализация БД...");

        if (RemoveBefore)
            await RemoveAsync(Cancel).ConfigureAwait(false);

        // если миграций нет. не устраивает
        //await _db.Database.EnsureCreatedAsync(Cancel).ConfigureAwait(false);
        _Logger.LogInformation("Применение миграции БД...");
        await _db.Database.MigrateAsync(Cancel).ConfigureAwait(false);
        _Logger.LogInformation("Применение миграции БД выполнено");

        if (AddTestData)
        {
            await InitializeProductAsync(Cancel);
            await InitializeEmployeesAsync(Cancel);
        }

        await InitializeIdentityAsync(Cancel);

        _Logger.LogInformation("Инициализация БД выполнена успешно");
    }

    private async Task InitializeProductAsync(CancellationToken Cancel)
    {
        _Logger.LogInformation("Инициализация БД тестовыми данными...");

        if (await _db.Products.AnyAsync(Cancel).ConfigureAwait(false))
        {
            _Logger.LogInformation("Инициализация БД тестовыми данными не требуется");
            return;
        }

        var sections_pool = TestData.Sections.ToDictionary(s => s.Id);
        var brands_pool = TestData.Brands.ToDictionary(b => b.Id);

        foreach (var child_section in TestData.Sections.Where(s => s.ParentId is not null))
        {
            //child_section.Parent = sections_pool[Convert.ToInt32(child_section.ParentId)];
            // тоже самое, но сделал как у преподавателя
            child_section.Parent = sections_pool[(int)child_section.ParentId!];
        }

        foreach (var product in TestData.Products)
        {
            product.Section = sections_pool[product.SectionId];
            // если у товара установлен бренд
            if (product.BrandId is { } brand_id)
                product.Brand = brands_pool[brand_id];

            product.Id = 0;
            product.SectionId = 0;
            product.BrandId = null;
        }

        foreach (var brand in TestData.Brands)
            brand.Id = 0;

        foreach (var sections in TestData.Sections)
        {
            sections.Id = 0;
            sections.ParentId = null;
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(Cancel);

        _Logger.LogInformation("Добавление данных в БД...");
        await _db.Sections.AddRangeAsync(TestData.Sections, Cancel);
        await _db.SaveChangesAsync(Cancel);

        await _db.Brands.AddRangeAsync(TestData.Brands, Cancel);
        await _db.SaveChangesAsync(Cancel);

        await _db.Products.AddRangeAsync(TestData.Products, Cancel);
        await _db.SaveChangesAsync(Cancel);
        _Logger.LogInformation("Добавление данных в БД выполнено успешно");

        #region ExpiredCode
        //_Logger.LogInformation("Добавление в БД секций...");
        //await _db.Sections.AddRangeAsync(TestData.Sections, Cancel);

        //// сырой sql запрос для включения возможность добавления тестовых данных с нашими идентификаторами
        //await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] ON", Cancel);
        //await _db.SaveChangesAsync(Cancel);
        //// выключаем этот режим
        //await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] OFF", Cancel);
        //_Logger.LogInformation("Добавление в БД секций выполнено успешно");

        //_Logger.LogInformation("Добавление в БД брендов...");
        //await _db.Brands.AddRangeAsync(TestData.Brands, Cancel);

        //await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] ON", Cancel);
        //await _db.SaveChangesAsync(Cancel);        
        //await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] OFF", Cancel);
        //_Logger.LogInformation("Добавление в БД брендов выполнено успешно");

        //_Logger.LogInformation("Добавление в БД товаров...");
        //await _db.Products.AddRangeAsync(TestData.Products, Cancel);

        //await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] ON", Cancel);
        //await _db.SaveChangesAsync(Cancel);
        //await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] OFF", Cancel);
        #endregion

        await transaction.CommitAsync(Cancel);
        _Logger.LogInformation("Транзакция в БД завершена");

    }

    private async Task InitializeEmployeesAsync(CancellationToken Cancel)
    {
        if (await _db.Employees.AnyAsync(Cancel).ConfigureAwait(false))
        {
            _Logger.LogInformation("Инициализация таблицы сотрудников в БД не требуется");
            return;
        }

        _Logger.LogInformation("Инициализация БД сотрудников...");
        foreach (var employee in TestData.Employees)
            employee.Id = 0;

        await _db.AddRangeAsync(TestData.Employees, Cancel);
        await _db.SaveChangesAsync(Cancel);
        _Logger.LogInformation("Инициализация БД сотрудников выполнена успешно");
    }

    private async Task InitializeIdentityAsync(CancellationToken Cancel)
    {
        _Logger.LogInformation("Инициализация БД системы Identity...");

        async Task CheckRoleAsync(string RoleName)
        {
            if (await _RoleManager.RoleExistsAsync(RoleName))
                _Logger.LogInformation("Роль {0} существует в БД", RoleName);
            else
            {
                _Logger.LogInformation("Роль {0} отсутсвует в БД. Создаю...", RoleName);
                await _RoleManager.CreateAsync(new Role { Name = RoleName });
                _Logger.LogInformation("Роль {0} успешно создана", RoleName);
            }
        }

        await CheckRoleAsync(Role.Administrators);
        await CheckRoleAsync(Role.Users);

        if (await _UserManager.FindByNameAsync(User.Administrator) is null)
        {
            _Logger.LogInformation("Пользователь {0} отсутсвует в БД. Создаю...", User.Administrator);

            var admin = new User
            {
                UserName = User.Administrator
            };

            var creation_result = await _UserManager.CreateAsync(admin, User.AdminPassword);
            if (creation_result.Succeeded)
            {
                _Logger.LogInformation("Пользователь {0} в БД cоздан. Присвоение роли Администратор...", User.Administrator);

                await _UserManager.AddToRoleAsync(admin, Role.Administrators);

                _Logger.LogInformation("Пользователю {0} присвоена роль Администратор", User.Administrator);
            }
            else
            {
                var errors = creation_result.Errors.Select(e => e.Description);
                var error_message = string.Join(", ", errors);
                _Logger.LogError("Учётная запись {0} не может быть создана. Ошибка: {1}", User.Administrator, error_message);

                throw new InvalidOperationException("Невозможно создать {User.Administrator}. Ошибка: {error_message}");
            }
        }
        else
        {
            _Logger.LogInformation("Пользователь {0} существует", User.Administrator);
        }

        _Logger.LogInformation("Инициализация БД системы Idnity выполнена успешно");
    }
}


