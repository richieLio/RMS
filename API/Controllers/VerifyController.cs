using BussinessObject.Services.VerifyServices;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/verify")]
    [ApiController]
    public class VerifyController : ControllerBase
    {

        private readonly IVerifyServices _Verify;

        public VerifyController(IVerifyServices Verify)
        {
            _Verify = Verify;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string Email)
        {
            ResultModel result = await _Verify.SendOTPEmailRequest(Email);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] UserVerifyOTPReqModel VerifyModel)
        {
            ResultModel result = await _Verify.VerifyOTPCode(VerifyModel.Email, VerifyModel.OTPCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
