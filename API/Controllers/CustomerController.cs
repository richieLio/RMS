using BussinessObject.Services.CustomerServices;
using DataAccess.Entities;
using DataAccess.Models.CustomerModel;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginReqModel LoginForm)
        {
            ResultModel result = await _customerServices.Login(LoginForm);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("verify-2nd-pass")]
        public async Task<IActionResult> VerifySecondPass([FromBody] SecondPassVerificationReqModel secondPassVerificationModel)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            ResultModel result = await _customerServices.VerifySecondPass(token, secondPassVerificationModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] CustomerUpdateModel updateModel)
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
            ResultModel result = await _customerServices.UpdateUserProfile(userId,updateModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile(Guid customerId)
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
            ResultModel result = await _customerServices.GetCustomerProfile(userId, customerId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
