using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Services.Services;

public class CartService : ICartService
{
    private readonly IProductData _ProductData;
    private readonly ICartStore _СartStore;

    public CartService(IProductData ProductData, ICartStore CartStore)
    {        
        _ProductData = ProductData;
        _СartStore = CartStore;
    }

    public void Add(int Id)
    {
        var cart = _СartStore.Cart;
        cart.Add(Id);
        _СartStore.Cart = cart;
    }

    public void Decrement(int Id)
    {
        var cart = _СartStore.Cart;
        cart.Decrement(Id);
        _СartStore.Cart = cart;
    }

    public void Remove(int Id)
    {
        var cart = _СartStore.Cart;
        cart.Remove(Id);
        _СartStore.Cart = cart;
    }

    public void Clear()
    {
        var cart = _СartStore.Cart;
        cart.Clear();
        _СartStore.Cart = cart;
    }

    public CartViewModel GetViewModel()
    {
        var cart = _СartStore.Cart;

        var products = _ProductData.GetProducts(new()
        {
            Ids = cart.Items.Select(item => item.ProductId).ToArray(),
        });

        // перегоняем во вьюмодели и в словарь по идентификаторам
        var products_views = products.ToView().ToDictionary(p => p!.Id);


        // формируем вью модель
        return new()
        {
            Items = cart.Items     // только те, который есть в словаре
             .Where(item => products_views.ContainsKey(item.ProductId))
             .Select(item => (products_views[item.ProductId], item.Quantity))!,
        };
    }
}
