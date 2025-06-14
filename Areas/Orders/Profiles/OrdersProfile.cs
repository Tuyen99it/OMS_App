using AutoMapper;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Areas.Orders.Dtos;
using OMS_App.Areas.Orders.Models;

namespace OMS_App.Areas.Orders.Profiles
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            // order mapping
            CreateMap<Order, OrderReadDto>();
            CreateMap<OrderCreateDto, Order>();

            CreateMap<OrderUpdateDto, Order>();

            //ordered product mapping

            CreateMap<OrderedProduct, OrderedProductReadDto>();
            CreateMap<OrderedProductCreateDto, OrderedProduct>();
            CreateMap<ProductName, OrderedProduct>().ForMember(desk => desk.Id, opt => opt.Ignore())
                                                    .ForMember(desk => desk.ProductName, opt => opt.MapFrom(src => src.Name))
                                                    .ForMember(desk => desk.TotalProduct, opt => opt.Ignore())
                                                    .ForMember(desk => desk.TotalPrices, opt => opt.Ignore());

            // OrderAddress mapping
            CreateMap<OrderAddress, OrderAddressReadDto>();
            CreateMap<OrderAddressCreateDto, OrderAddress>();

            CreateMap<OrderAddressUpdateDto, OrderAddress>();
           

        }
    }
}