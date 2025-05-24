using AutoMapper;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Profiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<ProductName, ProductNameReadDto>().ForMember(desk => desk.Quantity, opt => opt.MapFrom(src => src.ProductInventories.Count()));
            CreateMap<ProductNameCreateDto, ProductInventory>()
                                            .ForMember(desk => desk.Id, opt => opt.Ignore())
                                            .ForMember(desk => desk.DateCreate, opt => opt.MapFrom(src => src.CreateDate))
                                             .ForMember(desk => desk.ExpireDate, opt => opt.MapFrom(src => src.ExpireDate));
            CreateMap<ProductName, ProductNameUpdateDto>();                
                                           

            CreateMap<ProductNameCreateDto, ProductName>()
                                            .ForMember(desk => desk.Id, opt => opt.Ignore())
                                            .ForMember(desk => desk.ProductImages, opt => opt.Ignore())
                                            .ForMember(desk => desk.CategoriesProduct, opt => opt.Ignore());
            CreateMap<ProductNameUpdateDto, ProductName>()
                                            .ForMember(desk => desk.ProductImages, opt => opt.Ignore())
                                            .ForMember(desk => desk.CategoriesProduct, opt => opt.Ignore());
        }
    }
}