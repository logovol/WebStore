using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers;

public class CartController : Controller
{
    private readonly ICartService _CartService;

    public CartController(ICartService cartService) => _CartService = cartService;
    
    public IActionResult Index() => View(new CartOrderViewModel { Cart = _CartService.GetViewModel() }); // cart.html

    public IActionResult Add(int Id)
    {
        _CartService.Add(Id);
        return RedirectToAction("Index", "Cart");
    }

    public IActionResult Decrement(int Id)
    {
        _CartService.Decrement(Id);
        return RedirectToAction("Index", "Cart");
    }

    public IActionResult Remove(int Id)
    {
        _CartService.Remove(Id);
        return RedirectToAction("Index", "Cart");
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(OrderViewModel OrderModel, [FromServices] IOrderService OrderService)
    {
        if (OrderModel is null)
            throw new ArgumentNullException(nameof(OrderModel));

        if (!ModelState.IsValid)
            return View(nameof(Index), new CartOrderViewModel
            {
                Cart = _CartService.GetViewModel(),
                Order = OrderModel,
            });

        var order = await OrderService.CreateOrderAsync(
            User.Identity!.Name!,
            _CartService.GetViewModel(),
            OrderModel);

        _CartService.Clear();

        return RedirectToAction(nameof(OrderConfirmed), new { order.Id });
    }

    public IActionResult OrderConfirmed(int Id)
    {
        ViewBag.OrderId = Id;
        return View();
    }

    #region WebAPI
    public IActionResult GetCartView() => ViewComponent("Cart");

    public IActionResult AddAPI(int Id)
    {
        _CartService.Add(Id);
        // отправка сообщения скрипту
        return Json(new { Id, message = $"Товар {Id} был добавлен в корзину" });
    }

    public IActionResult DecrementAPI(int Id)
    {
        _CartService.Decrement(Id);
        // отправка сообщения скрипту, можно JSON и OK
        return Ok(new { Id, message = $"Количество товара {Id} было уменьшено на 1" });
    }

    public IActionResult RemoveAPI(int Id)
    {
        _CartService.Remove(Id);
        // отправка сообщения скрипту, можно JSON и OK
        return Ok(new { Id, message = $"Товар {Id} был удалён из корзины" });
    }
    #endregion
}
