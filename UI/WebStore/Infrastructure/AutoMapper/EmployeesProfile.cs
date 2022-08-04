using AutoMapper;

using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;

namespace WebStore.Infrastructure.AutoMapper
{
    public class EmployeesProfile : Profile
    {
        public EmployeesProfile()
        {
            // карта проекции: источник и назначение
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(m => m.Name, o => o.MapFrom(e => e.Name))
                .ReverseMap();
        }

        public class ProductProfile : Profile
        { 
            public ProductProfile()
            {
                CreateMap<Product, ProductViewModel>();
                
            }
        }
    }
}
