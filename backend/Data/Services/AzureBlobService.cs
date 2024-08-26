using Azure.Storage;
using Azure.Storage.Blobs;
using backend.Models;
using backend.Options;
using Microsoft.Extensions.Options;

namespace backend.Data.Services
{
    public class AzureBlobService
    {
        private readonly string _storageAccount = "pfala";
        private AppConfOptions _appConfOptions {  get; }
        private readonly string _key;
        private readonly BlobContainerClient _fileContainer;

        public AzureBlobService(IConfiguration configuration, IOptionsSnapshot<AppConfOptions> options)
        {
            _appConfOptions = options.Value;
            _key = _appConfOptions.BlobKey;
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _fileContainer = blobServiceClient.GetBlobContainerClient("potholes");
        }

        public async Task<List<BlobDto>> ListBlobs()
        {
            List<BlobDto> files = new List<BlobDto>();

            await foreach (var file in _fileContainer.GetBlobsAsync())
            {
                string uri = _fileContainer.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new BlobDto { Name = name, Uri = fullUri, ContentType = file.Properties.ContentType });
            }

            return files;
        }

        public async Task<BlobResponseDto> UploadImage(IFormFile blob)
        {
            string id = GenerateId();
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string fileName = $"{id}-{timestamp}";
            BlobResponseDto response = new();
            BlobClient client = _fileContainer.GetBlobClient(fileName);

            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {fileName} uploaded successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response;
        }

        public async Task<BlobResponseDto> DeleteImage(string filename)
        {
            BlobClient file = _fileContainer.GetBlobClient(filename);

            await file.DeleteAsync();

            return new BlobResponseDto { Error = false, Status = $"File {filename} has been deleted successfully" };
        }

        private string GenerateId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
