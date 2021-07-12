namespace Demo.DDD.WithEFCore.API.Controllers
{
    using Demo.DDD.WithEFCore.API.Extensions;
    using Demo.DDD.WithEFCore.Data.Repositories;
    using Demo.DDD.WithEFCore.Entities;
    using Humanizer;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /* If we don't want versioning here, we can implement as a header value */
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> orderRepo;

        public OrdersController(IRepository<Order> orderRepo)
        {
            this.orderRepo = orderRepo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) 
        {
            return this.Ok(await Task.FromResult(new Order { Id = 123 }));
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

            if (orders is null || !orders.Any()) 
            {
                // ToDo: NotFound object without a message fails unit text "Assert.IsType<NotFoundObjectResult>(...)"
                return NotFound($"We didn't find any records. Page Number: {pageNumber}, Page Size: {pageSize}");
            }

            return Ok(orders);
        }

        [HttpPut("id")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] DTO.Order order) 
        {
            // We can do this validation globally as well.
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            /*
             * Avoid returning business domain entities directly via API calls. Domain entities:
             *  - Often include more data than the client requires.
             *  - Unnecessarily couple the app's internal domain model with the publicly exposed API.
             */

            // 200 (OK) or 204 (No Content)
            return Ok(await Task.FromResult(order));
        }

        /// <summary>
        /// Payload example: 
        /// [
        ///     {"op":"add", "path":"/note", "value":"some new value here"}
        /// ]
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("id")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] JsonPatchDocument<DTO.Order> patchDocument) 
        {
            if (patchDocument is null) 
            { 
                return BadRequest($"{nameof(patchDocument)} cannot be null.");
            }

            // Does a record exist?
            var order = await this.orderRepo.FindAsync(o => o.Id == id);

            if (order is null || order.Count == 0) 
            {
                return NotFound($"No order with id# {id} found in the system.");
            }

            // ToDo: Use AutoMapper
            var orderToPatch = order[0].ToDto();
            patchDocument.ApplyTo(orderToPatch);

            // ToDo: Use AutoMapper to convert DTO to Entity (Domain/Database entity) and save
            var orderEntity = orderToPatch.ToEntity();

            return NoContent();
        }
        
        [NonAction]
        public async Task<IActionResult> Get(string message) 
        {
            throw new InvalidOperationException(message);
        }
    }
}
