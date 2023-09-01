using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Domain.ViewModels
{
    public class CatalogViewModel
    {
        public int? SectionId { get; init; }
        public int? BrandId { get; init; }

        public IEnumerable<ProductViewModel> Products { get; init; } = Enumerable.Empty<ProductViewModel>();

        public PageViewModel PageModel { get; init; } = null!;
    }
}
