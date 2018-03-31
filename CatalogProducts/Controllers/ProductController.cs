using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogProducts.Models;
using CatalogProducts.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CatalogProducts.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 4;

        public ProductController(IProductRepository repository) => this.repository = repository;

        public ViewResult List(string category, int productPage = 1)
        {
            var products = repository.Products.Where(p => category == null || p.Category == category)
                                              .OrderBy(p => p.ProductID)
                                              .Skip((productPage - 1) * PageSize)
                                              .Take(PageSize);

            ProductsListViewModel productsListViewModel = new ProductsListViewModel
            {
                Products = products,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                        repository.Products.Count() : repository.Products.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            };

            return View(productsListViewModel);
        }

        public ViewResult Search(string stringSearch, int productPage = 1)
        {
            stringSearch = stringSearch ?? "";
            string s = stringSearch.ToUpper();
            ViewBag.StringSearch = stringSearch;
            var products = repository.Products.Where(p => p.Name.ToUpper().Contains(s))
                                              .OrderBy(p => p.ProductID);

            ProductsListViewModel productsListViewModel = new ProductsListViewModel
            {
                Products = products.Skip((productPage - 1) * PageSize)
                                   .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = products.Count()
                }
            };

            return View(productsListViewModel);
        }
    }
}
