namespace Demo.DDD.WithEFCore.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.IO;

    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {

        private readonly ILogger<FilesController> _logger;

        public FilesController(ILogger<FilesController> logger)
        {
            this._logger = logger;
        }

        [HttpGet("{fileName}")]
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
