using IdentityServer.API1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IdentityServer.API1.Controllers
{
    [Route("api/[controller]/[action]")] //action ile metot isimleriyle endpoint'e ulaşmak istiyoruz.
    [ApiController]
    public class ProductController : ControllerBase
    {
        //-->   api/product/getproducts
        [Authorize(Policy = "ReadProduct")] //claim bazlı yetkilendirme yaparız. startup tarafında bunu geçeriz.
        [HttpGet]
        public IActionResult GetProducts()
        {
            var productList = new List<Product>() { 
                new Product { Id = 1, Name = "kalem 1", Price = 100, Stock = 100 },
                new Product { Id = 2, Name = "kalem 2", Price = 200, Stock = 200 }, 
                new Product { Id = 3, Name = "kalem 3", Price = 300, Stock = 300 },
                new Product { Id = 4, Name = "kalem 4", Price = 400, Stock = 400 }, 
                new Product { Id = 5, Name = "kalem 5", Price = 500, Stock = 500 } 
            };
            return Ok(productList);
        }

        [Authorize(Policy = "UpdateOrCreate")]
        public IActionResult UpdateProduct(int id)
        {
            return Ok($"id'si {id} olan ürün başarıyla güncellenmiştir.");
        }
        [Authorize(Policy = "UpdateOrCreate")]
        public IActionResult CreateProduct(Product product)
        {
            return Ok(product);
        }
    }
}
