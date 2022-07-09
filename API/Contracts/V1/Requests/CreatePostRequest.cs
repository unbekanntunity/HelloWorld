using Microsoft.AspNetCore.Mvc;

namespace API.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string? Content { get; set; }
        public List<string> TagNames { get; set; } = new List<string>();
        public List<IFormFile> RawImages { get; set; } = new List<IFormFile>();
    }
}
