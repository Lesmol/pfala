using backend.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/v1/blob")]
    [ApiController]
    public class AzureBlobController : ControllerBase
    {
        private readonly AzureBlobService _service;

        public AzureBlobController(AzureBlobService service)
        {
            _service = service;
        }

        [HttpGet("list-blobs")]
        public async Task<IActionResult> ListBlobs()
        {
            var result = await _service.ListBlobs();

            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await _service.UploadImage(file);

            return Ok(result);
        }

        [HttpDelete("filename")]
        public async Task<IActionResult> DeleteImage(string filename)
        {
            var result = await _service.DeleteImage(filename);

            return Ok(result);
        }
    }
}
