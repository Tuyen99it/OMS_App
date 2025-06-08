using Microsoft.Extensions.DependencyInjection;


namespace OMS_App.Areas.Orders.Data
{
    public static class Program
    {
        public static void AddOrdersServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepo, OrdereServiceRepo>();
            services.AddScoped<IOrderAddressRepo, OrderAddressServiceRepo>();
            services.AddScoped<IOrderedProductRepo, OrderedProductServiceRepo>();

        }
    }
}