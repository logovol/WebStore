using Microsoft.AspNetCore.Mvc;
using WebStore.Infrastructure.Mapping;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices] IProductData ProductData)
        {
            var products = ProductData.GetProducts().OrderBy(p => p.Order).Take(6).ToView();
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
