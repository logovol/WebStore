﻿using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Interfaces.Services;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients;

public class ProductsClient : BaseClient, IProductData
{
    public ProductsClient(HttpClient Client) : base(Client, "api/products")
    {
    }

    public Brand? GetBrandById(int Id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Brand> GetBrands()
    {
        throw new NotImplementedException();
    }

    public Product? GetProductById(int Id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Product> GetProducts(ProductFilter? Filter = null)
    {
        throw new NotImplementedException();
    }

    public Section? GetSectionById(int Id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Section> GetSections()
    {
        throw new NotImplementedException();
    }
}
