namespace HelloWorldAPI.Services
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

            var fileName = Guid.NewGuid().ToString();

            using (Stream fileStream = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return Path.Combine(directory, fileName);
        }

        public string GetImageUrl(string relativeImagePath)
        {
            if (File.Exists(Path.Combine(Path.Combine(_basePath, "Images", relativeImagePath))))
            {
                return new Uri(_baseDomain + "/Images/" + relativeImagePath).ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
