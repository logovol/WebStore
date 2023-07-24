﻿using Microsoft.AspNetCore.Mvc;

using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components;

public class BrandsViewComponent : ViewComponent
{

    private readonly IProductData _ProductData;

    public BrandsViewComponent(IProductData ProductData) => _ProductData = ProductData;

    // можно выбирать представление, если их несколько
    // public IViewComponentResult Invoke() => View("Name");
    public IViewComponentResult Invoke(string BrandId) => View(GetBrands());

    private IEnumerable<BrandViewModel> GetBrands() =>
        _ProductData.GetBrands()
        .OrderBy(b => b.Order)
        .Select(b => new BrandViewModel
        {
            Id = b.Id,
            Name = b.Name,
        });
}
