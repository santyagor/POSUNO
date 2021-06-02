using Microsoft.EntityFrameworkCore;
using POSUNO.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POSUNO.Api.Data
{
    public class SeedDb
    {
        public readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckUserAsync();
            await CheckCustomersAsync();
            await CheckProductsAsync();

        }

        private async Task CheckUserAsync()
        {
            if(!_context.Users.Any())
            {
                _context.Users.Add(new User {Email = "santyagor@outlook.com",FirsName = "Santiago", LastName="Cepeda", Password="123456" });
                _context.Users.Add(new User { Email = "luchitog@hotmail.es", FirsName = "Lucho", LastName = "Garcia", Password = "123456" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCustomersAsync()
        {
            if (!_context.Customers.Any())
            {
                User user = await _context.Users.FirstOrDefaultAsync();
                for (int i = 1; i < 50; i++)
                {
                    _context.Customers.Add(new Customer { FirtsName = $"Cliente {i}", LastName = $"Apellido {i}",Phonenumber ="0998984439",Email=$"cliente{i}@gmail.com", Address="Calle 100",IsActive=true, User = user });
                }
                await _context.SaveChangesAsync();
            }
        }
        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                Random random = new Random();
                User user = await _context.Users.FirstOrDefaultAsync();
                for (int i = 1; i < 200; i++)
                {
                    _context.Products.Add(new Product {Name=$"Producto {i}", Description=$"Producto {i}",Price = random.Next(5,1000),Stock= random.Next(0,500),IsActive=true, User= user  });
                }                                
                await _context.SaveChangesAsync();
            }
        }

        

    }
}
