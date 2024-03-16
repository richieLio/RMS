using BussinessObject.Services.CloudStorageServices;
using BussinessObject.Services.ContractServices;
using BussinessObject.Utilities;
using DataAccess.Entities;
using DataAccess.Models.ContractModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/contract")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractServices _contractServices;
        private readonly ICloudStorageServices _cloudStorageServices;
        private readonly CloudStorage _cloudStorage;
        public ContractController(IContractServices contractServices, ICloudStorageServices cloudStorageServices, CloudStorage cloudStorage)
        {
            _contractServices = contractServices;
            _cloudStorageServices = cloudStorageServices;
            _cloudStorage = cloudStorage;
        }

        [HttpGet("list")] // fixed (only display contracts belong to that userid(owner)) 
        public async Task<IActionResult> GetContractList(int page)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            ResultModel result = await _contractServices.GetContractList(userId, page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("information")] // fixed
        public async Task<IActionResult> GetContractInformation(Guid contractId)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            ResultModel result = await _contractServices.GetContractInformation(userId,contractId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

      

        [HttpPut("update-status")] //fixed
        public async Task<IActionResult> Update([FromBody] ContractUpdateStatusReqModel Form)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            ResultModel result = await _contractServices.UpdateContractStatus(userId, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("contract-of-room")] // fixed
        public async Task<IActionResult> GetContractByRoom(Guid roomId)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            ResultModel result = await _contractServices.GetContractByRoom(userId, roomId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPut("file-upload")]  // fixed
        public async Task<IActionResult> UploadFile([FromForm] ContractReqModel Form)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }

            string filePath = $"Contract/{Form.Id}/images/{Form.ImagesUrl.FileName}";
            await _cloudStorage.UploadFile(Form.ImagesUrl, filePath);
            
            ResultModel result = await _contractServices.UpdateContract(userId, Form, filePath);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }



    }
}
