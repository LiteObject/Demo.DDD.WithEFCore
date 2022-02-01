namespace Demo.DDD.WithEFCore.API.Controllers
{
    using Demo.DDD.WithEFCore.API.DTO;
    using Demo.DDD.WithEFCore.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IEnumerable<Product> _products = new List<Product> { 
            new Product() { Id =1, Name = "Product One", UnitPrice = 100 },
            new Product() { Id =2, Name = "Product Two", UnitPrice = 200 }
        };

        private readonly IEnumerable<IDiscountService> _discountServices;

        public ProductsController(IEnumerable<IDiscountService> discountServices)
        {
            this._discountServices = discountServices;

            // Example code:
            var specialService = (this._discountServices.FirstOrDefault( s => s.GetType() == typeof(SpecialDiscountService)) as SpecialDiscountService);
            var test = specialService.ApplySpecialSprice(400);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = new List<Product>();

            foreach (var product in _products) 
            {
                var price = product.UnitPrice;

                foreach (var discount in _discountServices) 
                {
                    price = discount.Apply(price);                    
                }

                product.UnitPrice = price;
                result.Add(product);
            }

            return Ok(result);
        }
    }
}