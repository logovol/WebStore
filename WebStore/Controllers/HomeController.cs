using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //return View();
            return Content("Index action");
        }

        public IActionResult Greetings(string? id)
        {
            return Content($"Hello from first controller - {id}");
        }
    }
}
