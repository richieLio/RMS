using BussinessObject.Utilities;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.CloudStorageServices
{
    public class CloudStorageServices : ICloudStorageServices
    {
        private readonly CloudStorage _cloudStorage;
        public CloudStorageServices(CloudStorage cloudStorage)
        {
            _cloudStorage = cloudStorage;
        }
        public async Task<ResultModel> UploadFile(IFormFile formFile, string filePath)
        {
            ResultModel result = new ResultModel();
            await _cloudStorage.UploadFile(formFile, filePath);
            return result;
        }
    }
}
