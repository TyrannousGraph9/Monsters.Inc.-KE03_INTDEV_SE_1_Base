using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class MatrixIncDbInitializer
    {
        public static void Initialize(MatrixIncDbContext context)
        {
            // Look for any customers.
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            // TODO: Hier moet ik nog wat namen verzinnen die betrekking hebben op de matrix.
            // - Denk aan de m3 boutjes, moertjes en ringetjes.
            // - Denk aan namen van schepen
            // - Denk aan namen van vliegtuigen         

            var customers = new Customer[]
            {
                new Customer { Name = "Neo", Address = "123 Elm St" , Active=true},
                new Customer { Name = "Morpheus", Address = "456 Oak St", Active = true },
                new Customer { Name = "Trinity", Address = "789 Pine St", Active = true }
            };
            context.Customers.AddRange(customers);

            var orders = new Order[]
            {
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-01-01")},
                new Order { Customer = customers[0], OrderDate = DateTime.Parse("2021-02-01")},
                new Order { Customer = customers[1], OrderDate = DateTime.Parse("2021-02-01")},
                new Order { Customer = customers[2], OrderDate = DateTime.Parse("2021-03-01")}
            };  
            context.Orders.AddRange(orders);

            var parts = new Part[]
            {
                new Part { Name = "Tandwiel", Description = "Overdracht van rotatie in bijvoorbeeld de motor of luikmechanismen"},
                new Part { Name = "M5 Boutje", Description = "Bevestiging van panelen, buizen of interne modules"},
                new Part { Name = "Hydraulische cilinder", Description = "Openen/sluiten van zware luchtsluizen of bewegende onderdelen"},
                new Part { Name = "Koelvloeistofpomp", Description = "Koeling van de motor of elektronische systemen."}
            };

            context.Parts.AddRange(parts);

            var categories = new Category[]
            {
                new Category { CategoryName = "Schepen" },
                new Category { CategoryName = "Wapens" },
                new Category { CategoryName = "Onderdelen" }
            };
            context.Categories.AddRange(categories);

            var products = new Product[]
            {
                new Product { Name = "Nebuchadnezzar", Description = "Het schip waarop Neo voor het eerst de echte wereld leert kennen", Price = 10000.00m, Category = categories[0], Image = System.IO.File.ReadAllBytes("./wwwroot/Images/Nebuchadnezzar.webp") },
                new Product { Name = "Jack-in Chair", Description = "Stoel met een rugsteun en metalen armen waarin mensen zitten om ingeplugd te worden in de Matrix via een kabel in de nekpoort", Price = 500.50m, Category = categories[2], Image = System.IO.File.ReadAllBytes("./wwwroot/Images/stoel ofzo.jpg") },
                new Product { Name = "EMP (Electro-Magnetic Pulse) Device", Description = "Wapentuig op de schepen van Zion", Price = 129.99m, Category = categories[1], Image = System.IO.File.ReadAllBytes("./wwwroot/Images/bOOM.webp") }
            };
            context.Products.AddRange(products);

            context.SaveChanges();

            var orderItems = new OrderItem[]
            {
                new OrderItem { Order = orders[0], Product = products[0], Quantity = 1, UnitPrice = products[0].Price },
                new OrderItem { Order = orders[0], Product = products[1], Quantity = 2, UnitPrice = products[1].Price },
                new OrderItem { Order = orders[1], Product = products[2], Quantity = 2, UnitPrice = products[2].Price },
                new OrderItem { Order = orders[2], Product = products[1], Quantity = 3, UnitPrice = products[1].Price },
                new OrderItem { Order = orders[3], Product = products[2], Quantity = 1, UnitPrice = products[2].Price }
            };
            context.OrderItems.AddRange(orderItems);

            context.SaveChanges();

            context.Database.EnsureCreated();

        }
    }
}
