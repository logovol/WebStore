using System.Diagnostics.CodeAnalysis;

using WebStore.Domain.Entities;

namespace WebStore.Domain.DTO;

public class ProductDTO
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public int Order { get; init; }

    public decimal Price { get; init; }

    public string ImageUrl { get; init; } = null!;

    public SectionDTO Section { get; init; } = null!;

    public BrandDTO? Brand { get; init; }
}

public class SectionDTO
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public int Order { get; init; }

    public int? ParentId { get; init; }
}

public class BrandDTO
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public int Order { get; init; }
    
    //public int ProductsCount { get; init; }

    // взамен закоментаренного
    public IEnumerable<int> ProductIds { get; init; } = null!;
}


// 1. Проекции

// 1.1 для брендов
public static class BrandDTOMapper
{
    [return: NotNullIfNotNull("brand")] // если brand !null, то результат - !null
    public static BrandDTO? ToDTO(this Brand? brand) => brand is null
        ? null
        : new()
        {
            Id = brand.Id,
            Name = brand.Name,
            Order = brand.Order,
            //ProductsCount = brand.Products.Count,
            ProductIds = brand.Products.Select(p => p.Id),
        };

    [return: NotNullIfNotNull("brand")] // если brand !null, то результат - !null
    public static Brand? FromDTO(this BrandDTO? brand) => brand is null
        ? null
        : new()
        {
            Id = brand.Id,
            Name = brand.Name,
            Order = brand.Order,
            //Products = new Product[brand.ProductsCount],
            Products = brand.ProductIds.Select(id => new Product { Id = id }).ToArray(),
        };

    public static IEnumerable<BrandDTO> ToDTO(this IEnumerable<Brand>? brands) => brands?.Select(ToDTO)!;
    public static IEnumerable<Brand> FromDTO(this IEnumerable<BrandDTO>? brands) => brands?.Select(FromDTO)!;
}

// 1.2 для секций
public static class SectionDTOMapper
{
    [return: NotNullIfNotNull("section")] // если section !null, то результат - !null
    public static SectionDTO? ToDTO(this Section? section) => section is null
        ? null
        : new()
        {
            Id = section.Id,
            Name = section.Name,
            Order = section.Order,
            ParentId = section.ParentId,
        };

    [return: NotNullIfNotNull("section")] // если section !null, то результат - !null
    public static Section? FromDTO(this SectionDTO? section) => section is null
        ? null
        : new()
        {
            Id = section.Id,
            Name = section.Name,
            Order = section.Order,
            ParentId = section.ParentId,
        };

    public static IEnumerable<SectionDTO> ToDTO(this IEnumerable<Section>? sections) => sections?.Select(ToDTO)!;
    public static IEnumerable<Section> FromDTO(this IEnumerable<SectionDTO>? sections) => sections?.Select(FromDTO)!;
}

// 1.2 для товаров
public static class ProductDTOMapper
{
    [return: NotNullIfNotNull("product")] // если product !null, то результат - !null
    public static ProductDTO? ToDTO(this Product? product) => product is null
        ? null
        : new()
        {
            Id = product.Id,
            Name = product.Name,
            Order = product.Order,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Brand = product.Brand.ToDTO(),
            Section = product.Section.ToDTO(),
        };

    [return: NotNullIfNotNull("product")] // если product !null, то результат - !null
    public static Product? FromDTO(this ProductDTO? product) => product is null
        ? null
        : new()
        {
            Id = product.Id,
            Name = product.Name,
            Order = product.Order,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Brand = product.Brand.FromDTO(),
            Section = product.Section.FromDTO(),
        };

    public static IEnumerable<ProductDTO> ToDTO(this IEnumerable<Product>? products) => products?.Select(ToDTO)!;
    public static IEnumerable<Product> FromDTO(this IEnumerable<ProductDTO>? products) => products?.Select(FromDTO)!;

    [return: NotNullIfNotNull("page")]
    public static Page<ProductDTO>? ToDTO(this Page<Product>? page) => page is null
        ? null
        : new(page.Items.ToDTO(), page.PageNumber, page.PageSize, page.TotalCount);

    [return: NotNullIfNotNull("page")]
    public static Page<Product>? FromDTO(this Page<ProductDTO>? page) => page is null
        ? null
        : new(page.Items.FromDTO(), page.PageNumber, page.PageSize, page.TotalCount);
}