using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Repositories.OTPRepo;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<ResultModel> SendOTPEmailRequest(string Email)
        {
            ResultModel Result = new ResultModel();
            try
            {
                var User = await _UserRepo.GetUserByEmail(Email);
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
                Html = Html.Replace("{{toEmail}}", Email);
                bool check = await EmailUltilities.SendEmail(Email, "Reset Password", Html);
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

        public async Task<ResultModel> VerifyOTPCode(string Email, string OTPCode)
        {
            ResultModel Result = new();
            try
            {
                var User = await _UserRepo.GetUserByEmail(Email);
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The User cannot validate to verify this OTP";
                    return Result;
                }
                var GetOTP = await _OTPRepo.GetOTPByUserId(User.Id);
                if (GetOTP != null)
                {
                    if (GetOTP.IsUsed || (DateTime.Now - GetOTP.CreatedAt).TotalMinutes > 10)
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "The OTP is expired!";
                        return Result;
                    }
                    GetOTP.IsUsed = true;
                    _ = await _OTPRepo.Update(GetOTP);
                    User.Status = UserStatus.RESETPASSWORD;
                    _ = await _UserRepo.Update(User);
                    Result.IsSuccess = true;
                    Result.Code = 200;
                }
                else
                {
                    Result.IsSuccess = true;
                    Result.Code = 400;
                    Result.Message = "The OTP is invalid!";
                }
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }
    }   
}
