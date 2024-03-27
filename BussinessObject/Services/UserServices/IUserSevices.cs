using DataAccess.Models.EmailModel;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.UserServices
{
    public interface IUserSevices
    {

        public Task<ResultModel> Login(UserLoginReqModel LoginForm);

        public Task<ResultModel> ResendVerifyOTP(UserResendOTPReqModel RegisterForm);
        public Task<ResultModel> CreateAccount(UserReqModel RegisterForm);
   
        Task<ResultModel> GetUserProfile(Guid userId);
        Task<ResultModel> UpdateUserProfile(UserUpdateModel updateModel);
        Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel);
        public Task<ResultModel> ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel);
        Task<ResultModel> VerifyEmail(EmailVerificationReqModel verificationModel);

        Task<ResultModel> CreateOrLoginWithFacebook(string accessToken);
        Task<ResultModel> CreateOrLoginWithGoogle(string credential);
    }
}
