using DataAccess.ResultModel;
using DataAccess.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Models.EmailModel;

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

    }
}
