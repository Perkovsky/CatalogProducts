using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogProducts.Models
{
    public static class SeedData
    {
        private static IEnumerable<Product> LoadData()
        {
            IEnumerable<Product> products = new List<Product>();
            using (var sr = new StreamReader(@"wwwroot\loaddata\catalog.csv"))
            using (var csv = new CsvReader(sr, new Configuration { Delimiter = ";" }))
                products = csv.GetRecords<Product>().ToList();

            return products;
        }

        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            if (!context.Products.Any())
            {
                TextReader textReader = File.OpenText(@"wwwroot\loaddata\catalog.csv");
                var csv = new CsvReader(textReader);
                var records = csv.GetRecords<Product>();

                context.Products.AddRange(LoadData());
                context.SaveChanges();
            }
        }
    }
}
