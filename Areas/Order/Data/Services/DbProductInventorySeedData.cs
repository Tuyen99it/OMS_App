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

        for (int i = 0; i <= 100; i++)
        {
            productInventories.Add(new ProductNameCreateDto()
            {
                Name = "Sample: " + i.ToString(),
                Description = "Mieu ta: " + i.ToString(),
                CreateDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddDays(1000),
                Price = (1000 + i)
            });

        }
        var products = mapper.Map<List<ProductName>>(productInventories);
        _context.ProductNames.AddRange(products);
        _context.SaveChanges();
        Console.WriteLine("--> Seed data for ProductInventory successfully...");

    }
    public static void InitializeProductInventory(OMSDBContext _context, IMapper mapper)
    {
        var productsName = _context.ProductNames.ToList();
        if (productsName != null)
        {
            foreach (var productName in productsName)
            {
                Console.WriteLine("--> Starting seed goods for ProductInventory...");


                var goods = new List<ProductInventory>();

                for (int i = 0; i <= 10; i++)
                {
                    goods.Add(new ProductInventory()
                    {
                        ProductNameId = productName.Id,
                        DateCreate = DateTime.Now,
                        ExpireDate = DateTime.Now.AddDays(1000),

                    });

                }

                _context.ProductInventories.AddRange(goods);
                _context.SaveChanges();
                Console.WriteLine("--> Seed goods for ProductInventory successfully...");

            }

        }
    }
}

