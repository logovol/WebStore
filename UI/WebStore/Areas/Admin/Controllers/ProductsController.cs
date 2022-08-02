using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities.Identity;
using WebStore.Services.Interfaces;

namespace WebStore.Areas.Admin.Controllers;

[Authorize(Roles = Role.Administrators)]
public class ProductsController : Controller
{
    private readonly IProductData _ProductData;
    private readonly ILogger<ProductsController> _Logger;

    public ProductsController(IProductData ProductData, ILogger<ProductsController> Logger)
    {
        _ProductData = ProductData;
        _Logger = Logger;
    }
    public IActionResult Index()
    {
        var products = _ProductData.GetProducts();
        return View(products);
    }

    public IActionResult Edit(int id)
    {
        // заглушка
        return View();
        // logovol ht
        // 1. Добавить в IProductData метод Edit и Delete
        // 2. В сервисах InMemoryProductData и SqlProductData реализовать эти методы
        // 3. Во вьюхе Edit вызвать этот метод и отредактировать по route-Id
    }

    public IActionResult Delete(int id)
    {
        // заглушка
        return View();
        // logovol ht
        // 1. Добавить в IProductData метод Edit и Delete
        // 2. В сервисах InMemoryProductData и SqlProductData реализовать эти методы
        // 3. 3. Во вьюхе Delete вызвать этот метод и удалить по route-Id
    }
}
