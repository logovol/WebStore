using Microsoft.EntityFrameworkCore;

using WebStore.Domain.Entities;

namespace WebStore.DAL.Context;

// класс контекста базы данных
public class WebStoreDB : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Section> Section { get; set; } = null!;

    public WebStoreDB(DbContextOptions<WebStoreDB> options) : base(options)
    {

    }
    
    // переопределение метода
    // это и так есть без нашего участия

    //protected override void OnModelCreating(ModelBuilder db)
    //{
    //    base.OnModelCreating(db);

    //    //db.Entity<Section>()
    //    //    .HasMany(s => s.Products)
    //    //    .WithOne(p => p.Section)
    //    //    .OnDelete(DeleteBehavior.Cascade);
            
    //}    
        
}
