namespace Demo.DDD.WithEFCore.API.Controllers
{
    using Demo.DDD.WithEFCore.Data.Repositories;
    using Demo.DDD.WithEFCore.Entities;
    using Humanizer;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> orderRepo;

        public OrdersController(IRepository<Order> orderRepo)
        {
            this.orderRepo = orderRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10) 
        {
            if (pageSize > 100)
            { 
                pageSize = 100;
            }

            var orders = await orderRepo.GetAllAsync(pageNumber, pageSize);

            return Ok(orders);
        }
    }
}
