using Microsoft.EntityFrameworkCore;

using WebStore.DAL.Context;

namespace WebStore.Data;

// класс занимается инициализацией БД
public class DbInitializer
{
    private readonly WebStoreDB _db;
    private readonly ILogger<DbInitializer> _Logger;
    
    public DbInitializer(WebStoreDB db, ILogger<DbInitializer> Logger)
    {
        _db = db;
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

    public async Task InitializeAsync(bool RemoveBefore,CancellationToken Cancel = default)
    {
        _Logger.LogInformation("Инициализация БД...");
        
        if (RemoveBefore)
            await RemoveAsync(Cancel).ConfigureAwait(false);

        // если миграций нет. не устраивает
        //await _db.Database.EnsureCreatedAsync(Cancel).ConfigureAwait(false);
        _Logger.LogInformation("Применение миграции БД...");
        await _db.Database.MigrateAsync(Cancel).ConfigureAwait(false);
        _Logger.LogInformation("Применение миграции БД выполнено");

        await InitializeProductAsync(Cancel);
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
                
        await using var transaction = await _db.Database.BeginTransactionAsync(Cancel);

        _Logger.LogInformation("Добавление в БД секций...");
        await _db.Sections.AddRangeAsync(TestData.Sections, Cancel);

        // сырой sql запрос для включения возможность добавления тестовых данных с нашими идентификаторами
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] ON", Cancel);
        await _db.SaveChangesAsync(Cancel);
        // выключаем этот режим
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Sections] OFF", Cancel);
        _Logger.LogInformation("Добавление в БД секций выполнено успешно");

        _Logger.LogInformation("Добавление в БД брендов...");
        await _db.Brands.AddRangeAsync(TestData.Brands, Cancel);
                
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] ON", Cancel);
        await _db.SaveChangesAsync(Cancel);        
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Brands] OFF", Cancel);
        _Logger.LogInformation("Добавление в БД брендов выполнено успешно");

        _Logger.LogInformation("Добавление в БД товаров...");
        await _db.Products.AddRangeAsync(TestData.Products, Cancel);

        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] ON", Cancel);
        await _db.SaveChangesAsync(Cancel);
        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Products] OFF", Cancel);

        await transaction.CommitAsync(Cancel);

    }
}


