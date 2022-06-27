using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using WebStore.Domain.Entities.Base;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities;

// уникальные имены не нужны
[Index(nameof(Name), IsUnique = false)]
public class Section : NamedEntity, IOrderedEntity
{
    public int Order { get; set; }
    public int? ParentId { get; set; }

    // можно не писать EF сама создаст
    [ForeignKey(nameof(ParentId))]
    // навигационное свойство
    public Section? Parent { get; set; }

    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
}
