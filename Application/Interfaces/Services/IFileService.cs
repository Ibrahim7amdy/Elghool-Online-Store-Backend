using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile? file, string folderName, string? existingFileUrl = null);
        Task DeleteFileAsync(string fileUrl);
    }
}