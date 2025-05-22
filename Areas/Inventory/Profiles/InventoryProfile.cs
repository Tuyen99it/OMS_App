using AutoMapper;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Inventory.Models;

namespace OMS_App.Areas.Inventory.Profiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<ProductInventory, InventoryProductReadDto>();
            CreateMap<InventoryProductCreateDto, ProductInventory>()
                                            .ForMember(desk => desk.Id, opt => opt.Ignore())
                                            .ForMember(desk => desk.ProductImages, opt => opt.Ignore())
                                            .ForMember(desk => desk.CategoriesProduct, opt => opt.Ignore());
            CreateMap<InventoryProductUpdateDto, ProductInventory>()
                                            .ForMember(desk => desk.ProductImages, opt => opt.Ignore())
                                            .ForMember(desk => desk.CategoriesProduct, opt => opt.Ignore());
        }
    }
}