using API.Contracts.V1;
using API.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Services
{
    public class ImageController : Controller
    {
        private readonly IFileManager _fileManager;

        public ImageController(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        [HttpGet(ApiRoutes.Image.Get)]
        public async Task<IActionResult> GetImageFile([FromRoute] string userId, [FromRoute] Guid id)
        {
            var response = _fileManager.GetImage(userId, id);
            return Ok(new Response<FileContentResult>(new FileContentResult(response, "image/png")));
        }
    }
}
