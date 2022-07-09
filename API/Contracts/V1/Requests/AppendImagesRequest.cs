namespace API.Contracts.V1.Requests
{
    public class AppendImagesRequest
    {
        public List<IFormFile> RawImages { get; set; }
    }
}
