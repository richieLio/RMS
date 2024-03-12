using DataAccess.Entities;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Authorization;
using BussinessObject.Services.UserServices;
using DataAccess.Models.EmailModel;

namespace API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserSevices _user;

        public UserController(IUserSevices user)
        {
            _user = user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReqModel LoginForm)
        {
            ResultModel result = await _user.Login(LoginForm);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] UserReqModel Form)
        {
            ResultModel result = await _user.CreateAccount(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
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
            var result = await _user.GetUserProfile(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserUpdateModel updateModel)
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
            ResultModel result = await _user.UpdateUserProfile(updateModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReqModel changePasswordModel)
        {
            var userIdString = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }

            ResultModel result = await _user.ChangePassword(userId, changePasswordModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordReqModel ResetPasswordReqModel)
        {
            ResultModel result = await _user.ResetPassword(ResetPasswordReqModel);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationReqModel verificationModel)
        {
            try
            {
                ResultModel result = await _user.VerifyEmail(verificationModel);
                return result.IsSuccess ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPut("resend-verify-email-otp")]
        public async Task<IActionResult> ResendVerifyEmail([FromBody] UserResendOTPReqModel Form)
        {
            ResultModel result = await _user.ResendVerifyOTP(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}
