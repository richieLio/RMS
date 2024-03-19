using BussinessObject.Services.VerifyServices;
using DataAccess.Models.UserModel;
using DataAccess.Models.VerifyModel;
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
        public async Task<IActionResult> SendOtp([FromBody] SendOTPReqModel sendOTPReqModel)
        {
            ResultModel result = await _Verify.SendOTPEmailRequest(sendOTPReqModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] UserVerifyOTPResModel VerifyModel)
        {
            if (string.IsNullOrEmpty(VerifyModel.Email) || string.IsNullOrEmpty(VerifyModel.OTPCode))
            {
                return BadRequest("Email and OTP code are required.");
            }
            ResultModel result = await _Verify.VerifyOTPCode(VerifyModel.Email, VerifyModel.OTPCode);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
