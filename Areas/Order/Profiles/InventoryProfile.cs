using AutoMapper;
using OMS_App.Areas.Orders.Dtos;
using OMS_App.Areas.Orders.Models;

namespace OMS_App.Areas.Orders.Profiles
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            CreateMap<Order, OrderReadDto>();
            CreateMap<OrderCreateDto, Order>();

            CreateMap<OrderUpdateDto, Order>();

            CreateMap<OrderedProduct, OrderedProductReadDto>();
            CreateMap<OrderedProductCreateDto, OrderedProduct>();

            CreateMap<OrderAddress, OrderAddressReadDto>();
            CreateMap<OrderAddressCreateDto, OrderAddress>();

            CreateMap<OrderAddressUpdateDto, OrderAddress>();
           

        }
    }
}