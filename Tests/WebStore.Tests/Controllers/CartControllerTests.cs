﻿using System;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

using Moq;

using WebStore.Controllers;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers;

[TestClass]
public class CartControllerTests
{
    [TestMethod]
    public async Task CheckOut_ModelState_Invalid_Returns_View_with_Model()
    {
        const string expected_description = "Test description";

        var cart_service_mock = new Mock<ICartService>();
        var order_service_mock = new Mock<IOrderService>();
        
        var controller = new CartController(cart_service_mock.Object);
        controller.ModelState.AddModelError("", "Test invalid model");
        
        var order_model = new OrderViewModel
        {
            Description = expected_description,
        };

        var result = await controller.Checkout(order_model, order_service_mock.Object);

        var view_result = Assert.IsType<ViewResult>(result);

        var model = Assert.IsAssignableFrom<CartOrderViewModel>(view_result.Model);

        Assert.Equal(expected_description, model.Order.Description);

        cart_service_mock.Verify(s => s.GetViewModel());
        cart_service_mock.VerifyNoOtherCalls();
        order_service_mock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task CheckOut_ModelState_Valid_Call_Service_and_Returns_Redirect()
    {
        const string expected_user = "Test user";

        const string expected_description = "Test description";
        const string expected_address = "Test address";
        const string expected_phone = "Test phone";

        var cart_service_mock = new Mock<ICartService>();
        cart_service_mock
            .Setup(c => c.GetViewModel())
            .Returns(
                new CartViewModel
                {
                    Items = new[] { ( new ProductViewModel { Name = "Test product" }, 1) }
                });

        const int expected_order_id = 1;
        var order_service_mock = new Mock<IOrderService>();
        order_service_mock
            .Setup(c => c.CreateOrderAsync(It.IsAny<string>(), It.IsAny<CartViewModel>(), It.IsAny<OrderViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Order
            {
                Id = expected_order_id,
                Description = expected_description,
                Address = expected_address,
                Phone = expected_phone,
                Date = DateTime.Now,
                Items = Array.Empty<OrderItem>(),
            });


        var controller = new CartController(cart_service_mock.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, expected_user) }))
                }
            }
        };

        var order_model = new OrderViewModel
        {
            Address = expected_address,
            Phone = expected_phone,
            Description = expected_description,
        };

        var result = await controller.Checkout(order_model, order_service_mock.Object);

        var redirect_result = Assert.IsType<RedirectToActionResult>(result);

        Assert.Null(redirect_result.ControllerName);
        Assert.Equal(nameof(CartController.OrderConfirmed), redirect_result.ActionName);

        Assert.Equal(expected_order_id, redirect_result.RouteValues!["id"]);

        order_service_mock.Verify(s => s.CreateOrderAsync(It.Is<string>(user => user == expected_user), It.IsAny<CartViewModel>(), It.IsAny<OrderViewModel>(), It.IsAny<CancellationToken>()));
        cart_service_mock.Verify(s => s.Clear());
        cart_service_mock.Verify(s => s.GetViewModel());

        order_service_mock.VerifyNoOtherCalls();
        cart_service_mock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task Checkout_thrown_ArgumentNullException_when_OrderModel_is_null_1()
    {
        var cart_service_mock = new Mock<ICartService>();
        var order_service_mock = new Mock<IOrderService>();

        var controller = new CartController(cart_service_mock.Object);

        Exception? error = null;
        try
        {
            _ = await controller.Checkout(null!, order_service_mock.Object);
        }
        catch (Exception e)
        {
            error = e;
        }
        
        var argument_null_exception = Assert.IsType<ArgumentNullException>(error);
        Assert.Equal("OrderModel", argument_null_exception.ParamName);
    }

    [TestMethod, ExpectedException(typeof(ArgumentNullException))]
    public async Task Checkout_thrown_ArgumentNullException_when_OrderModel_is_null_2()
    {
        var cart_service_mock = new Mock<ICartService>();
        var order_service_mock = new Mock<IOrderService>();

        var controller = new CartController(cart_service_mock.Object);        
        _ = await controller.Checkout(null!, order_service_mock.Object);        
    }

    [TestMethod]
    public async Task Checkout_thrown_ArgumentNullException_when_OrderModel_is_null_3()
    {
        var cart_service_mock = new Mock<ICartService>();
        var order_service_mock = new Mock<IOrderService>();

        var controller = new CartController(cart_service_mock.Object);
        var argument_null_exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.Checkout(null!, order_service_mock.Object));

        Assert.Equal("OrderModel", argument_null_exception.ParamName);
    }
}
