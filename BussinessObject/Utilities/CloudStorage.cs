using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Utilities
{
    public class CloudStorage
    {
        private readonly StorageClient _storage;
        private string bucketName = "fir-3ad0b.appspot.com";


        public CloudStorage(string serviceAccountKeyPath)
        {
            GoogleCredential credential = GoogleCredential.FromFile(serviceAccountKeyPath);
            _storage = StorageClient.Create(credential);
        }
        public async Task<string> UploadFile(IFormFile file, string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var objectName = $"{filePath}/{file.FileName}";

                _storage.UploadObject(bucketName, objectName, file.ContentType, memoryStream);

                return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{objectName}";
            }
        }
        public async Task<byte[]> DownloadFileFromFirebase(string filePath, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                string objectName = $"{filePath}/{fileName}";
                await _storage.DownloadObjectAsync(bucketName, objectName, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }
        }


    }
}
