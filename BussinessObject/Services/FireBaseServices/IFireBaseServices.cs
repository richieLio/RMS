using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BussinessObject.Services.FireBaseServices
{
    public interface IFireBaseServices
    {
        public Task<string> UploadFile(IFormFile file);

    }
}
