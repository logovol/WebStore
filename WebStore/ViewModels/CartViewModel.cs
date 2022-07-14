namespace WebStore.ViewModels;

public class CartViewModel
{
    public IEnumerable<(ProductViewModel Product, int Quantity)> items { get; set; } = null!;

    public int ItemsCount => items.Sum(item => item.Quantity);
    
    public decimal TotalPrice => items.Sum(item => item.Quantity * item.Product.Price);
}
