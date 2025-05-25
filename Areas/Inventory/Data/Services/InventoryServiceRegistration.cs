using Microsoft.Extensions.DependencyInjection;


namespace OMS_App.Areas.Inventory.Data
{
    public static class Program
    {
        public static void AddInventoryServices(this IServiceCollection services)
        {
            services.AddScoped<IProductNameRepo, ProductNameServiceRepo>();
            services.AddScoped<IProductCategoryRepo, ProductCategoryServiceRepo>();
            services.AddScoped<IInventoryImageRepo, InventoryImageServiceRepo>();
            services.AddScoped<IProductInventoryRepo, ProductInventoryServiceRepo>();
        }
    }
}