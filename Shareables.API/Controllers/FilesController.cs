using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shareables.API.DTO;

namespace Shareables.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private static readonly IEnumerable<string> AllowedExtensions = new List<string>
        {
            ".jpg", ".jpeg", ".png"
        };

        [Authorize]
        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            var path = Path.Combine("wwwroot", "temp", fileName);

            return Ok(new { exists = System.IO.File.Exists(path) });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromForm] FileUploadDto dto)
        {
            var extension = Path.GetExtension(dto.File.FileName);

            if (!AllowedExtensions.Contains(extension))
            {
                return new UnsupportedMediaTypeResult();
            }

            var originalFileName = Path.GetFileName(dto.File.FileName);
            var savePath = Path.Combine("wwwroot", "temp", originalFileName);

            using var fs = new FileStream(savePath, FileMode.Create);
            dto.File.CopyTo(fs);

            return StatusCode(201, new { file = originalFileName });
        }
    }
}
