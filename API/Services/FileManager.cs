using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;

namespace API.Services
{
    public class FileManager : IFileManager
    {
        private readonly string _basePath;
        private readonly string _baseDomain;

        public FileManager(string basePath, string baseDomain)
        {
            _basePath = basePath;
            _baseDomain = baseDomain;
        }

        public async Task<string> SaveImageAsync(string dirName, IFormFile image)
        {
            var directory = Path.Combine(_basePath, "Images", dirName);

            Directory.CreateDirectory(directory); 
            var fileName = Guid.NewGuid().ToString() + "." + image.ContentType.Split("/").Last();

            using (Stream fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return Path.Combine("Images", dirName, fileName);
        }

        public string GetImageUrl(string imagePath)
        {
            var fullPath = Path.Combine(_basePath, imagePath);

            return File.Exists(fullPath) ? new Uri(Path.Combine(_baseDomain, imagePath)).ToString() : string.Empty;
        }

        public bool RemoveImage(string imagePath)
        {
            var fullPath = Path.Combine(_basePath, imagePath);

            if(File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public byte[] GetImage(string userId, Guid id)
        {
            var fullPath = Path.Combine(_basePath, Path.Combine("Images", userId, id.ToString() + ".png"));

            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }

            return Array.Empty<byte>();
        }
    }
}
