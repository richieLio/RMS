using BussinessObject.Services.ServiceFeeServices;
using DataAccess.Entities;
using DataAccess.Models.ServiceModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/servicefee")]
    [ApiController]
    public class ServiceFeeController : ControllerBase
    {

        private readonly IServiceFeeServices _serviceFeeServices;
        public ServiceFeeController(IServiceFeeServices serviceFeeServices)
        {
            _serviceFeeServices = serviceFeeServices;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllServices(int page)
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
            ResultModel result = await _serviceFeeServices.GetServicesList(userId, page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }
        [HttpPost("add-new")]
        public async Task<IActionResult> AddNewService(ServiceCreateReqModel service)
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
            ResultModel result = await _serviceFeeServices.AddNewService(userId, service);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateServices(ServiceUpdateReqModel service)
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
            ResultModel result = await _serviceFeeServices.UpdateService(userId, service);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteService(Guid serviceId)
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
            ResultModel result = await _serviceFeeServices.RemoveService(userId, serviceId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }
    }
}
