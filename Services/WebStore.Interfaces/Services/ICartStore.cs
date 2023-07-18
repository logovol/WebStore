using WebStore.Domain.Entities;

namespace WebStore.Interfaces.Services;

public class ICartStore
{
    public Cart Cart { get; set; }
}
