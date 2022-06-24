using Microsoft.AspNetCore.Mvc;

using WebStore.Domain;
using WebStore.Infrastructure.Mapping;
using WebStore.Services.Interfaces;
using WebStore.ViewModels;

namespace WebStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductData _ProductData;
        public CatalogController(IProductData productData) => _ProductData = productData;

        // Bind доступные свойства
        public IActionResult Index([Bind("BrandId,SectionId")]ProductFilter filter)
        {            
            var products = _ProductData.GetProducts(filter);

            return View(new CatalogViewModel
            {
                BrandId = filter.BrandId,
                SectionId = filter.SectionId,
                Products = products.OrderBy(p => p.Order).ToView()!,
            });
        }
    }
}
