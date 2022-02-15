using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.DDD.WithEFCore.API.Controllers.V1
{
    /// <summary>
    /// This is the legacy (v1.0) Values endpoint.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")] // To make veriosning work with header value
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// This is the legacy GET method.
        /// </summary>
        /// <returns>
        /// Endpoint version number.
        /// </returns>
        [HttpGet()]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            return Ok("V1");
        }
    }
}
