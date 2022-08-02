using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Services.InSQL;

public class SqlOrderService : IOrderService
{
    private readonly WebStoreDB _db;
    private readonly UserManager<User> _UserManager;
    private readonly ILogger<SqlOrderService> _Logger;

    public SqlOrderService(WebStoreDB db, UserManager<User> UserManager, ILogger<SqlOrderService> Logger)
    {
        _db = db;
        _UserManager = UserManager;
        _Logger = Logger;
    }

    public async Task<Order> CreateOrderAsync(string UserName, CartViewModel Cart, OrderViewModel OrderModel, CancellationToken Cancel = default)
    {
        var user = await _UserManager.FindByNameAsync(UserName).ConfigureAwait(false);

        if (user is null)
            throw new InvalidOperationException($"Пользователь с именем {UserName} в системе не найден");

        await using var transaction = await _db.Database.BeginTransactionAsync(Cancel).ConfigureAwait(false);

        var order = new Order
        {
            User = user,
            Address = OrderModel.Address,
            Phone = OrderModel.Phone,
            Description = OrderModel.Description,
        };

        var products_ids = Cart.Items.Select(item => item.Product.Id);

        var cart_products = await _db.Products
            .Where(p => products_ids.Contains(p.Id))
            .ToArrayAsync(Cancel);
        //.ConfigureAwait(false);

        // соединение Корзины с Товарами, полученными из БД
        order.Items = Cart.Items.Join(
            cart_products,
            cart_item => cart_item.Product.Id,
            cart_product => cart_product.Id,
            (cart_item, cart_product) => new OrderItem
            {
                Order = order,
                Product = cart_product,
                Price = cart_product.Price, // Можно добавить логику начисления скидок и т.д.
                Quantity = cart_item.Quantity,

            }).ToArray();

        // добавляем созданный заказ в БД
        await _db.Orders.AddAsync(order, Cancel);
        await _db.SaveChangesAsync(Cancel);

        await transaction.CommitAsync(Cancel);

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(int Id, CancellationToken Cancel = default)
    {
        var order = await _db.Orders
            .Include(order => order.User)
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == Id, Cancel)
            .ConfigureAwait(false);

        return order;
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string UserName, CancellationToken Cancel = default)
    {
        var orders = await _db.Orders
            .Include(order => order.User)
            .Include(order => order.Items)
            .Where(order => order.User.UserName == UserName)
            .ToArrayAsync(Cancel)
            .ConfigureAwait(false);

        // массив заказов по указанному пользователю
        return orders;
    }
}
