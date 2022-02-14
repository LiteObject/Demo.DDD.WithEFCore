using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.DDD.WithEFCore.API.Controllers.V2
{
    /// <summary>
    /// This is the latest (v2.0) Values endpoint.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// GET method
        /// </summary>
        /// <returns>
        /// Endpoint version number.
        /// </returns>
        [HttpGet()]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            return Ok("V2");
        }
    }
}
