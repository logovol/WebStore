using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebStore.Data;
using WebStore.Domain;
using WebStore.Domain.Entities;

namespace WebStore.Services.Interfaces
{
    public interface IProductData
    {
        IEnumerable<Section> GetSections();
        Section? GetSectionById(int Id);
        IEnumerable<Brand> GetBrands();
        Brand? GetBrandById(int Id);
        IEnumerable<Product> GetProducts(ProductFilter? Filter = null);
        Product? GetProductById(int Id);
    }
}
