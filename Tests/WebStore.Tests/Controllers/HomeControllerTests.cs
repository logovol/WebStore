﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Moq;

using WebStore.Controllers;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers;

[TestClass]
public class HomeControllerTests
{
    [TestMethod]
    public void Contacts_returns_with_View()
    {
        // AAA = A-A-A Arrange - Act - Assert
        // Подготовка данных - их сравнение и проверяем результат

        #region Arrange
        var controller = new HomeController(null!);
        #endregion

        #region Act
        var result = controller.Contacts();
        #endregion

        #region Assert
        var view_result = Assert.IsType<ViewResult>(result);

        // Assert.Equal(nameof(HomeController.Contacts), view_result.ViewName); // неправильное утверждениие
        Assert.Null(view_result.ViewName);
        #endregion
    }

    [TestMethod]
    public void Error404_returns_with_View()
    {
        // AAA = A-A-A Arrange - Act - Assert
        // Подготовка данных - их сравнение и проверяем результат

        #region Arrange
        var controller = new HomeController(null!);
        #endregion

        #region Act
        var result = controller.Error404();
        #endregion

        #region Assert
        var view_result = Assert.IsType<ViewResult>(result);

        Assert.Null(view_result.ViewName);
        #endregion
    }

    [TestMethod]
    public void Greetings_returns_with_Id_returns_with_Content_string_contains_42()
    {
        const string id = "42";
        var expecten_result_string = $"Hello from first controller - {id}";
        
        var controller = new HomeController(null!);        
        var result = controller.Greetings(id);
        
        var content_result = Assert.IsType<ContentResult>(result);
        var actual_result_string = content_result.Content;

        Assert.Equal(expecten_result_string, actual_result_string);
    }

    //private class TestProductData : IProductData
    //{
    //    public Brand? GetBrandById(int Id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<Brand> GetBrands()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Product? GetProductById(int Id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<Product> GetProducts(ProductFilter? Filter = null)
    //    {
    //        Assert.Null(Filter);
    //        return Enumerable.Empty<Product>();
    //    }

    //    public Section? GetSectionById(int Id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<Section> GetSections()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    [TestMethod]
    public void Index_returns_with_ViewBag_with_products()
    {
        //var products = Enumerable.Empty<Product>();
        const int total_count = 100;
        var products = Enumerable.Range(1, total_count).Select(id => new Product { Id = id, Name = $"Product-{id}", Section = new() { Name = "Section" } });
        var controller = new HomeController(null!);

        //var result = controller.Index(new TestProductData());

        var product_data_mock = new Mock<IProductData>();
        product_data_mock.Setup(s => s.GetProducts(It.IsAny<ProductFilter>()))
            .Returns(new Page<Product>(products, 1, total_count, total_count));

        var result = controller.Index(product_data_mock.Object);

        var view_result = Assert.IsType<ViewResult>(result);
        var actual_products_result = view_result.ViewData["Products"];
        var actual_products = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(actual_products_result);
        Assert.Equal(6, actual_products.Count());
        Assert.Equal(products.Select(p => p.Name).Take(6), actual_products.Select(p => p.Name));
        //Assert.Empty(actual_products);
    }
}
