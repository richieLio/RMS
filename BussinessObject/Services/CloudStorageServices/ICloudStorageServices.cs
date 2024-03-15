using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.CloudStorageServices
{
    public interface ICloudStorageServices
    {
        Task<ResultModel> UploadFile(IFormFile formFile, string filePath);
    }
}
