using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CatalogProducts.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogProducts.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext context;

        public AdminController(ApplicationDbContext context) => this.context = context;

        [HttpGet]
        public ViewResult Index() => View();

        [HttpPost]
        public async Task<ActionResult> Index(IFormFile file)
        {
            var products = await LoadDataAsync(file);
            InsertUpdateData(products);

            return RedirectToAction(nameof(Completed));
        }

        public ViewResult Completed() => View();

        private async Task<IEnumerable<Product>> LoadDataAsync(IFormFile file)
        {
            IEnumerable<Product> products = new List<Product>();
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] array = memoryStream.ToArray();
                using (var readStream = new StreamReader(new MemoryStream(array)))
                {
                    using (var csv = new CsvReader(readStream, new Configuration { Delimiter = ";" }))
                        products = csv.GetRecords<Product>().ToList();
                }
            }

            return products;
        }

        private void InsertUpdateData(IEnumerable<Product> products)
        {
            foreach (var item in products)
            {
                var productID = new SqlParameter("@productID", item.ProductID);
                var name = new SqlParameter("@name", item.Name);
                var category = new SqlParameter("@category", item.Category);
                var description = new SqlParameter("@description", item.Description);
                var inStock = new SqlParameter("@inStock", item.InStock);
                var price = new SqlParameter("@price", item.Price);

                context.Database.ExecuteSqlCommand("InsertUpdateCatalogProducts @productID, @name, @category, @description, @inStock, @price",
                    productID, name, category, description, inStock, price);
            }
        }
    }
}
