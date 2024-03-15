using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.FileUploadModel
{
    public class FileUploadModel 
    {
        public IFormFile file { get; set; }
    }
}
