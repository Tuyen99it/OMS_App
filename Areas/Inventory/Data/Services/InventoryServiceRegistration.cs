using Microsoft.Extensions.DependencyInjection;


namespace OMS_App.Areas.Inventory.Data
{
    public static class InventoryRegistration
    {
        public static void AddInventoryServices(this IServiceCollection services)
        {
            services.AddScoped<IProductInventoryRepo, ProductInventoryServiceRepo>();
            services.AddScoped<IProductCategoryRepo, ProductCategoryServiceRepo>();
            services.AddScoped<IInventoryImageRepo, InventoryImageServiceRepo>();
        }
    }
}