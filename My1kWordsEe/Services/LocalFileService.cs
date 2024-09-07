namespace My1kWordsEe.Services
{
    public class LocalFileService
    {
        private IWebHostEnvironment webHostEnvironment;
        private string relativePath;

        public LocalFileService(IWebHostEnvironment env, string relativePath)
        {
            this.webHostEnvironment = env;
            this.relativePath = relativePath;
        }

        public async Task Save(string fileName, Stream fileContents)
        {
            var uploads = Path.Combine(this.webHostEnvironment.WebRootPath, relativePath);
            {
                var filePath = Path.Combine(uploads, fileName);
                Directory.CreateDirectory(filePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileContents.CopyToAsync(fileStream);
                }
            }
        }
    }
}
