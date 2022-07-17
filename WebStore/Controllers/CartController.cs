using Microsoft.AspNetCore.Mvc;

using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _CartService;

        public CartController(ICartService cartService) => _CartService = cartService;
        
        public IActionResult Index() => View(_CartService.GetViewModel()); // cart.html
    }
}
