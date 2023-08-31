using Microsoft.AspNetCore.Mvc;

using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _Logger;
        public HomeController(ILogger<HomeController> Logger) => _Logger = Logger;

        public IActionResult Index([FromServices] IProductData ProductData)
        {
            var products = ProductData
                .GetProducts(new() { PageNumber = 1, PageSize = 6 })
                .Items
                .OrderBy(p => p.Order)                
                .ToView();
                //.Select(p => p.ToView());
            
            ViewBag.Products = products;

            return View();
            //return Content("Index action");
        }

        public IActionResult Greetings(string? id)
        {
            return Content($"Hello from first controller - {id}");
        }

        public IActionResult Contacts() => View();

        public IActionResult Error404() => View();
    }
}
