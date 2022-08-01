using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.Entities.Orders;

namespace WebStore.DAL.Context;

// класс контекста базы данных
public class WebStoreDB : IdentityDbContext<User, Role, string>
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Section> Sections { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    // public DbSet<OrderItem> можно не добавлять, EF добавит при разборе Orders
    // единственно сложнее будет делать запросы, пример запроса:

    public WebStoreDB(DbContextOptions<WebStoreDB> options) : base(options)
    {
        // запрос у объекта контекста БД без наличия public DbSet<OrderItem>
        // this.Set<OrderItem>().Where(item => item.Price > 100).OrderBy(i => i.Quantity).Take(5).ToArray();
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
