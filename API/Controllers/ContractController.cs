using BussinessObject.Services.ContractServices;
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

        public ContractController(IContractServices contractServices)
        {
            _contractServices = contractServices;
        }

        [HttpGet("list")]
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
            ResultModel result = await _contractServices.GetContractList(page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("information")]
        public async Task<IActionResult> GetRoomInformation(Guid contractId)
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
            ResultModel result = await _contractServices.GetContractInformation(contractId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ContractReqModel Form)
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
            ResultModel result = await _contractServices.UpdateContract(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-status")]
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
            ResultModel result = await _contractServices.UpdateContractStatus(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("contract-of-room")]
        public async Task<IActionResult> GetContractByRoom(Guid roomId)
        {
            ResultModel result = await _contractServices.GetContractByRoom(roomId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
