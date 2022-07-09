namespace API.Services
{
    public interface IFileManager
    {
        Task<string> SaveImageAsync(string dirName, IFormFile image);
        string GetImageUrl(string imagePath);
    }
}
