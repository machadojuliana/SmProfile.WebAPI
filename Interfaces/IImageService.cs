namespace SMProfile.WebAPI.Interfaces;

public interface IImageService
{
    Task<string?> UploadImageAsync(IFormFile file);
}