namespace Hotexper.Api.Services;

public interface IImageService
{
    Task UploadImage(string name, IFormFile file);
}