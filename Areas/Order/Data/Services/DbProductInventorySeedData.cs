using AutoMapper;
using Azure.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OMS_App.Areas.Inventory.Dtos;
using OMS_App.Areas.Inventory.Models;
using OMS_App.Areas.Orders.Models;
using OMS_App.Data;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OMS_App.Areas.Orders.Models;
using OMS_App.Models;


public static class DbOrdersSeedData
{
    public static void Initialize(OMSDBContext _context, IMapper mapper)
    {
        Console.WriteLine("--> Starting seed data for Oorder...");


        var ordersDto = new List<OrderCreateDto>();

        for (int i = 0; i <= 100; i++)
        {
            ordersDto.Add(new OrderCreateDto()
            {
               
                User = new AppUser
                {
                    FullName = "User" + i
                }
                
            });

        }
        var orders = mapper.Map<List<Order>>(ordersDto);
        _context.Orders.AddRange(orders);
        _context.SaveChanges();
        Console.WriteLine("--> Seed data for Orders successfully...");

    }
    public static void InitializeOrderAddressInventory(OMSDBContext _context, IMapper mapper)
    {
        var users = _context.AppUsers.ToList();
        if (users != null)
        {
            foreach (var user in users)
            {
                Console.WriteLine("--> Starting seed goods for orderAddress...");


                var addresses = new List<OrderAddress>();

                for (int i = 0; i <= 10; i++)
                {
                    addresses.Add(new OrderAddress()
                    {
                        Country = "Nước " + i,
                        Province = "Tỉnh" + i,
                        District = "Huyen" + i,
                       PhoneNumber="012345"+i

                    });

                }

                _context.OrderAddresses.AddRange(addresses);
                _context.SaveChanges();
                Console.WriteLine("--> Seed goods for OrderAddresses successfully...");

            }

        }
    }
}

