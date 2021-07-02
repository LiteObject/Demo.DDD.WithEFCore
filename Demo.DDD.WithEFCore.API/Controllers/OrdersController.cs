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
                // ToDo: Future requirment - we want to notify caller if page size is greater than 100.
                // return BadRequest(...);

                pageSize = 100;
            }

            var orders = await orderRepo.GetAllAsync(pageNumber, pageSize);

            return Ok(orders);
        }
    }
}
