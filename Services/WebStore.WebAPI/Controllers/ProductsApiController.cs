﻿using Microsoft.AspNetCore.Mvc;

using WebStore.Domain;
using WebStore.Domain.DTO;
using WebStore.Interfaces;
using WebStore.Interfaces.Services;

namespace WebStore.WebAPI.Controllers;

[ApiController]
[Route(WebApiAddresses.V1.Products)]
public class ProductsApiController : ControllerBase
{
    private readonly IProductData _ProductData;
    private readonly ILogger<ProductsApiController> _Logger;

    public ProductsApiController(IProductData ProductData, ILogger<ProductsApiController> Logger)
    {
        _ProductData = ProductData;
        _Logger = Logger;
    }

    [HttpGet("sections")]
    public IActionResult GetSections() => Ok(_ProductData.GetSections().ToDTO());


    [HttpGet("sections/{Id:int}")]
    public IActionResult GetSectionById(int Id) => _ProductData.GetSectionById(Id) is { } section
        ? Ok(section.ToDTO())
        : NotFound(new { Id });

    [HttpGet("brands")]
    public IActionResult GetBrands() => Ok(_ProductData.GetBrands().ToDTO());

    [HttpGet("brands/{Id:int}")]
    public IActionResult GetBrandById(int Id) => _ProductData.GetBrandById(Id) is { } brand
        ? Ok(brand.ToDTO())
        : NotFound(new { Id });

    [HttpPost]
    public IActionResult GetProducts([FromBody] ProductFilter filter)
    {
        var products = _ProductData.GetProducts(filter);

        if (products.TotalCount > 0)
            return Ok(products.ToDTO());
        return NoContent();
    }

    [HttpGet("{Id:int}")]
    public IActionResult GetProductById(int Id) =>_ProductData.GetProductById(Id) is { } product
        ? Ok(product.ToDTO())
        : NotFound(new { Id });
}
