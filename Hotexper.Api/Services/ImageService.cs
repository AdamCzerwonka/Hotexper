namespace Hotexper.Api.Services;

public class ImageService : IImageService
{
    public async Task UploadImage(string name, IFormFile file)
    {
        const string dirName = "Uploads";
        Directory.CreateDirectory(dirName);
        await using var fileStream = File.Open(Path.Combine(dirName, name + ".jpg"), FileMode.Create);
        await file.CopyToAsync(fileStream);
    }
}