namespace API.Services
{
    public interface IFileManager
    {
        Task<string> SaveImageAsync(string dirName, IFormFile image);
        string GetImageUrl(string imagePath);
        byte[] GetImage(string userId, Guid id);
        bool RemoveImage(string imagePath);
    }
}
