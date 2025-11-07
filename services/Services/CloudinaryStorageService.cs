using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using services.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace services.Services
{
    public class CloudinaryStorageService : ICloudinaryStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryStorageService> _logger;

        public CloudinaryStorageService(IConfiguration configuration, ILogger<CloudinaryStorageService> logger)
        {
            _logger = logger;

            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing. Please check appsettings.json");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder = "lesson-plans")
        {
            try
            {
                _logger.LogInformation("Uploading file {FileName} to Cloudinary folder {Folder}", fileName, folder);

                // Sanitize filename - remove special characters
                var sanitizedFileName = SanitizeFileName(fileName);

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(sanitizedFileName, fileStream),
                    Folder = folder,
                    PublicId = $"{folder}/{Path.GetFileNameWithoutExtension(sanitizedFileName)}_{DateTime.UtcNow.Ticks}",
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = false
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to upload file to Cloudinary: {uploadResult.Error?.Message}");
                }

                _logger.LogInformation("Successfully uploaded file to Cloudinary: {Url}", uploadResult.SecureUrl);

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Cloudinary");
                throw new Exception($"Failed to upload file to Cloudinary: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadWordDocumentAsync(Stream fileStream, string fileName)
        {
            return await UploadFileAsync(fileStream, fileName, "lesson-plans/documents");
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return false;
                }

                // Extract public ID from URL
                var publicId = ExtractPublicIdFromUrl(fileUrl);
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("Could not extract public ID from URL: {Url}", fileUrl);
                    return false;
                }

                return await DeleteFileByPublicIdAsync(publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Cloudinary");
                return false;
            }
        }

        public async Task<bool> DeleteFileByPublicIdAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrEmpty(publicId))
                {
                    return false;
                }

                _logger.LogInformation("Deleting file from Cloudinary: {PublicId}", publicId);

                var deletionParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Raw
                };

                var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                if (deletionResult.Result == "ok")
                {
                    _logger.LogInformation("Successfully deleted file from Cloudinary: {PublicId}", publicId);
                    return true;
                }

                _logger.LogWarning("Failed to delete file from Cloudinary: {Result}", deletionResult.Result);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Cloudinary");
                return false;
            }
        }

        #region Helper Methods

        private string SanitizeFileName(string fileName)
        {
            // Remove special characters and replace spaces with underscores
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return sanitized.Replace(" ", "_");
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            try
            {
                // Cloudinary URL format: https://res.cloudinary.com/{cloud_name}/{resource_type}/upload/{transformations}/{version}/{public_id}.{format}
                // We need to extract the public_id part

                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');

                // Find the 'upload' segment
                var uploadIndex = Array.FindIndex(segments, s => s.Equals("upload", StringComparison.OrdinalIgnoreCase));
                if (uploadIndex == -1 || uploadIndex >= segments.Length - 1)
                {
                    return string.Empty;
                }

                // Get all segments after 'upload' (skip version if exists)
                var publicIdSegments = segments.Skip(uploadIndex + 1).ToList();

                // Remove version segment if it starts with 'v' followed by numbers
                if (publicIdSegments.Count > 0 && publicIdSegments[0].StartsWith("v") && 
                    publicIdSegments[0].Length > 1 && char.IsDigit(publicIdSegments[0][1]))
                {
                    publicIdSegments.RemoveAt(0);
                }

                // Join remaining segments and remove file extension
                var publicIdWithExtension = string.Join("/", publicIdSegments);
                var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);

                // If there are folders, include them
                var lastSlashIndex = publicIdWithExtension.LastIndexOf('/');
                if (lastSlashIndex > 0)
                {
                    var folder = publicIdWithExtension.Substring(0, lastSlashIndex);
                    publicId = $"{folder}/{publicId}";
                }

                return publicId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting public ID from URL: {Url}", url);
                return string.Empty;
            }
        }

        #endregion
    }
}
