using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.UserModel;
using DataAccess.Models.VerifyModel;
using DataAccess.Repositories.OTPRepo;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using EmailUltilities = Business.Ultilities.Email;


namespace BussinessObject.Services.VerifyServices
{
    public class VerifyServices : IVerifyServices
    {
        private readonly IUserRepository _UserRepo;
        private readonly IOTPRepository _OTPRepo;

        public VerifyServices(IUserRepository UserRepo, IOTPRepository OTPRepo)
        {
            _UserRepo = UserRepo;
            _OTPRepo = OTPRepo;
        }

        private string CreateOTPCode()
        {
            Random rnd = new();
            return rnd.Next(100000, 999999).ToString();
        }

            public async Task<ResultModel> SendOTPEmailRequest(SendOTPReqModel sendOTPReqModel)
            {
                ResultModel Result = new ResultModel();
                try
                {
                    var User = await _UserRepo.GetUserByEmail(sendOTPReqModel.Email);
                    if (User == null)
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "The User with this email is invalid";
                        return Result;
                    }
                    var GetOTP = await _OTPRepo.GetOTPByUserId(User.Id);
                    if (GetOTP != null)
                    {
                        if ((DateTime.Now - GetOTP.CreatedAt).TotalMinutes < 2)
                        {
                            Result.IsSuccess = false;
                            Result.Code = 400;
                            Result.Message = "Can not send OTP right now!";
                            return Result;
                        }
                    }

                    string OTPCode = CreateOTPCode();
                    string FilePath = "../BussinessObject/TemplateEmail/ResetPassword.html";
                    string Html = File.ReadAllText(FilePath);
                    Html = Html.Replace("{{OTPCode}}", OTPCode);
                    Html = Html.Replace("{{toEmail}}", sendOTPReqModel.Email);
                    bool check = await EmailUltilities.SendEmail(sendOTPReqModel.Email, "Reset Password", Html);
                    if (!check)
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "Send email is failed!";
                        return Result;
                    }
                    Otpverify Otp = new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = User.Id,
                        OtpCode = OTPCode,
                        CreatedAt = DateTime.Now,
                        ExpiredAt = DateTime.Now.AddMinutes(10),
                        IsUsed = false
                    };
                    _ = await _OTPRepo.Insert(Otp);
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Message = "The OTP code has been sent to your email";
                }
                catch (Exception e)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
                }
                return Result;
            }

        public async Task<ResultModel> VerifyOTPCode(string email, string otpCode)
        {
            var result = new ResultModel();

            try
            {
                var user = await _UserRepo.GetUserByEmail(email);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User not found.";
                    return result;
                }

                var otp = await _OTPRepo.GetOTPByUserId(user.Id);
                if (otp == null || otp.IsUsed || (DateTime.Now - otp.CreatedAt).TotalMinutes > 10)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = otp == null ? "OTP not found." : "OTP expired or already used.";
                    return result;
                }

                if (otp.OtpCode != otpCode)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Incorrect OTP.";
                    return result;
                }

                otp.IsUsed = true;
                await _OTPRepo.Update(otp);

                user.Status = UserStatus.RESETPASSWORD;
                await _UserRepo.Update(user);

                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception e)
            {
               

                result.IsSuccess = false;
                result.Code = 500;
                result.Message = "An unexpected error occurred while verifying OTP.";
                result.ResponseFailed = e.Message;
            }

            return result;
        }
    }
}
