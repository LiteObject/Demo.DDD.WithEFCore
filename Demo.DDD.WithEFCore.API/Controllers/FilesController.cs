namespace Demo.DDD.WithEFCore.API.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")] // To make veriosning work with header value
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public FilesController(ILogger<FilesController> logger)
        {
            this._logger = logger;
        }

        [HttpGet("{fileName}")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetFile(string fileName)
        {
            this._logger.LogInformation($"{nameof(GetFile)} has been invoked with param(s): {nameof(fileName)}: {fileName}");

            fileName = "./Contents/my-text-file.txt";

            if (!System.IO.File.Exists(fileName))
            {
                return NotFound();
            }

            var bytes = System.IO.File.ReadAllBytes(fileName);
            return File(bytes, "text/plain", Path.GetFileName(fileName));
        }
    }
}
