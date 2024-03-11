using BussinessObject.Services.CustomerServices;
using DataAccess.Entities;
using DataAccess.Models.CustomerModel;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
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
    }
}
