using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface ICloudinaryStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder = "lesson-plans");
        Task<string> UploadWordDocumentAsync(Stream fileStream, string fileName);
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<bool> DeleteFileByPublicIdAsync(string publicId);
    }
}
