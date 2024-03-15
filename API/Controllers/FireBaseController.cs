using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BussinessObject.Services.FireBaseServices;
namespace API.Controllers
{
    [Route("api/storage")]
    [ApiController]
    public class FireBaseController : ControllerBase
    {
        private readonly FireBaseServices _firebaseStorageServices;


        public FireBaseController(FireBaseServices fireBaseServices)
        {
            _firebaseStorageServices = fireBaseServices;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<string> Upload(IFormFile formFile)
        {
            try
            {
                return await _firebaseStorageServices.UploadFile(formFile);
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
