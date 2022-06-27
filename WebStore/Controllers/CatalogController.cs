using AutoMapper;

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
        private readonly IMapper _Mapper;

        public CatalogController(IProductData productData, IMapper Mapper)
        {
            _ProductData = productData;
            _Mapper = Mapper;
        }

        // Bind доступные свойства
        public IActionResult Index([Bind("BrandId,SectionId")] ProductFilter filter)
        {            
            var products = _ProductData.GetProducts(filter);

            return View(new CatalogViewModel
            {
                BrandId = filter.BrandId,
                SectionId = filter.SectionId,
                Products = products.OrderBy(p => p.Order).Select(p => _Mapper.Map<ProductViewModel>(p)).ToList(),
                //Products = products.OrderBy(p => p.Order).ToView()!,
            });
        }
    }
}
