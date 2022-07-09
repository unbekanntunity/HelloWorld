using API.Domain.Database;

namespace API.Contracts.V1.Requests
{
    public class CreateProjectRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public List<string> TagNames { get; set; } = new List<string>();
        public List<string> MembersIds { get; set; } = new List<string>();

        public List<IFormFile> RawImages { get; set; } = new List<IFormFile>();
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
