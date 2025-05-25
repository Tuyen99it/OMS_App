using AutoMapper;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Data;

public static class DbProductInventorySeedData
{
    public static void Initialize(OMSDBContext _context, IMapper mapper)
    {
        Console.WriteLine("--> Starting seed data for ProductInventory...");


        var productInventories = new List<ProductNameCreateDto>();

        for (int i = 0; i <= 1000; i++)
        {
            productInventories.Add(new ProductNameCreateDto()
            {
                Name = "Sample: " + i.ToString(),
                Description = "Mieu ta: " + i.ToString(),
                CreateDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(1000),
                Price = 1000 + 1
            });

        }
        var products = mapper.Map<List<ProductName>>(productInventories);
        _context.ProductNames.AddRange(products);
        _context.SaveChanges();
        Console.WriteLine("--> Seed data for ProductInventory successfully...");
    }
}

