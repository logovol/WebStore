﻿using AutoMapper;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Controllers;

public class CatalogController : Controller
{
    private readonly IProductData _ProductData;
    private readonly IMapper _Mapper;
    private readonly IConfiguration _Configuration;

    public CatalogController(IProductData productData, IMapper Mapper, IConfiguration Configuration)
    {
        _ProductData = productData;
        _Mapper = Mapper;
        _Configuration = Configuration;
    }

    // Bind доступные свойства
    public IActionResult Index([Bind("BrandId,SectionId,PageNumber,PageSize")] ProductFilter filter)
    {
        filter.PageSize ??= int.TryParse(_Configuration["CatalogPageSize"], out var page_size) ? page_size : null;

        var products = _ProductData.GetProducts(filter);

        return View(new CatalogViewModel
        {
            BrandId = filter.BrandId,
            SectionId = filter.SectionId,
            Products = products
                .Items
                .OrderBy(p => p.Order)
                .Select(p => _Mapper.Map<ProductViewModel>(p)),
                //.Select(p => _Mapper.Map<ProductViewModel>(p)).ToList(),
                //Products = products.OrderBy(p => p.Order).ToView()!,
            PageModel = new()
            {
                Page = filter.PageNumber,
                PageSize = filter.PageSize ?? 0,
                TotalPages = products.PageCount,
            }
        });
    }

    public IActionResult Details(int Id)
    {
        var product = _ProductData.GetProductById(Id);
        if(product is null)
            return NotFound();

        return View(product.ToView());
    }

    // частичное предтавление страницы товаров для скрипта
    public IActionResult GetProductsAPI([Bind("BrandId,SectionId,PageNumber,PageSize")] ProductFilter filter)
    {
        var products = _ProductData.GetProducts(filter);
        return PartialView("Partial/_Products", products.Items.Select(p => _Mapper.Map<ProductViewModel>(p)));
    }
}
