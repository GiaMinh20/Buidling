using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IImageService
    {
        Task<DeletionResult> DeleteMediaAsync(string publicId);
        Task<ImageUploadResult> AddImageAsync(IFormFile file);
        Task<VideoUploadResult> AddVideoAsync(IFormFile file);
    }
}
