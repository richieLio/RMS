using AutoMapper;
using DataAccess.ResultModel;
using DataAccess.Models.UserModel;
using DataAccess.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DataAccess.Entities;
using Encoder = Business.Ultilities.Encoder;
using EmailUltilities = Business.Ultilities.Email;
using System.Threading.Tasks;
using DataAccess.Enums;
using Business.Ultilities;
using DataAccess.Models.EmailModel;
using MySqlX.XDevAPI.Common;

namespace BussinessObject.Services.UserServices
{
    public class UserSevices : IUserSevices
    {

        private readonly IUserRepository _userRepository;
        public UserSevices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<ResultModel> Login(UserLoginReqModel LoginForm)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(LoginForm.Email);
                if (User == null || User.Status == UserStatus.INACTIVE)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Please verify your account";
                    return Result;
                }
                else
                {
                    var Salt = User.Salt;
                    var PasswordStored = User.Password;
                    var Verify = Encoder.VerifyPasswordHashed(LoginForm.Password, Salt, PasswordStored);
                    if (Verify)
                    {
                        if (User.Status == UserStatus.RESETPASSWORD)
                        {
                            User.Status = UserStatus.ACTIVE;
                            _ = await _userRepository.Update(User);
                        }
                        UserLoginResModel LoginResData = new UserLoginResModel();

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<User, UserResModel>();
                        });
                        IMapper mapper = config.CreateMapper();
                        UserResModel UserResModel = mapper.Map<User, UserResModel>(User);

                        LoginResData.User = UserResModel;
                        LoginResData.Token = Encoder.GenerateJWT(User);
                        Result.IsSuccess = true;
                        Result.Code = 200;
                        Result.Data = LoginResData;
                        User.LastLoggedIn = DateTime.Now;
                        _ = await _userRepository.Update(User);
                    }
                    else
                    {
                        Result.IsSuccess = false;
                        Result.Code = 400;
                        Result.Message = "Password is invalid";
                    }
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
        public async Task<ResultModel> CreateAccount(UserReqModel RegisterForm)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(RegisterForm.Email);
                if (User != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Email is already registered!";
                }
                else
                {
                    string OTP = GenerateOTP();
                    DateTime expirationTime = DateTime.Now.AddMinutes(10);
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore());
                    });
                    IMapper mapper = config.CreateMapper();
                    User NewUser = mapper.Map<UserReqModel, User>(RegisterForm);
                    if (RegisterForm.Password == null)
                    {
                        RegisterForm.Password = Encoder.GenerateRandomPassword();
                    }
                    string FilePath = "../BussinessObject/TemplateEmail/FirstInformation.html";


                    string Html = File.ReadAllText(FilePath);
                    Html = Html.Replace("{{Email}}", RegisterForm.Email);
                    Html = Html.Replace("{{OTP}}", $"{OTP}");

                    bool emailSent = await EmailUltilities.SendEmail(RegisterForm.Email, "Email Verification", Html);

                    if (emailSent)
                    {
                        NewUser.Otp = OTP;
                        NewUser.Otpexpiration = expirationTime;
                        NewUser.Id = Guid.NewGuid();
                        NewUser.Status = UserStatus.INACTIVE;
                        NewUser.CreatedAt = DateTime.Now;
                        NewUser.Role = UserEnum.OWNER;
                        var HashedPasswordModel = Encoder.CreateHashPassword(RegisterForm.Password);
                        NewUser.Password = HashedPasswordModel.HashedPassword;
                        NewUser.Salt = HashedPasswordModel.Salt;

                        _ = await _userRepository.Insert(NewUser);

                        Result.IsSuccess = true;
                        Result.Code = 200;
                        Result.Message = "Verification email sent successfully!";
                    }
                    else
                    {
                        // Handle email sending failure
                        Result.IsSuccess = false;
                        Result.Code = 500;
                        Result.Message = "Failed to send verification email. Please try again later.";
                    }
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

        private string GenerateOTP()
        {
            Random rnd = new Random();
            int otp = rnd.Next(100000, 999999);
            return otp.ToString();
        }

        public async Task<ResultModel> GetUserProfile(Guid userId)
        {
            ResultModel Result = new();
            try
            {
                var user = await _userRepository.GetUserByID(userId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }

                var userProfile = new
                {
                    User_ID = user.Id,
                    user.FullName,
                    user.Email,
                    user.Address,
                    user.Dob,
                    user.Gender,
                    Phone = user.PhoneNumber,
                    user.Role,

                };

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = userProfile;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> UpdateUserProfile(UserUpdateModel updateModel)
        {
            ResultModel Result = new();
            try
            {
                var user = await _userRepository.GetUserByID(updateModel.Id);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }
                user.Email = updateModel.Email;
                user.PhoneNumber = updateModel.PhoneNumber;
                user.Address = updateModel.Address;
                user.Gender = updateModel.Gender;
                user.FullName = updateModel.FullName;


                _ = await _userRepository.Update(user);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Profile updated successfully";
            }
            catch (Exception ex)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.Message = "User doesn't have the required roles";

            }
            return Result;
        }

        public async Task<ResultModel> ChangePassword(Guid userId, ChangePasswordReqModel changePasswordModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.GetUserByID(userId);

                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404; // Not Found
                    result.Message = "User not found";
                    return result;
                }

                // Verify the old password
                var oldPasswordHash = user.Password;
                var oldPasswordSalt = user.Salt;
                var isOldPasswordValid = Encoder.VerifyPasswordHashed(changePasswordModel.OldPassword, oldPasswordSalt, oldPasswordHash);

                if (!isOldPasswordValid)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Old password is incorrect";
                    return result;
                }

                // Generate new password hash and salt
                var newPasswordHashModel = Encoder.CreateHashPassword(changePasswordModel.NewPassword);
                user.Password = newPasswordHashModel.HashedPassword;
                user.Salt = newPasswordHashModel.Salt;

                // Update the user in the database
                _ = await _userRepository.Update(user);

                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Password updated successfully";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel)
        {
            ResultModel Result = new();
            try
            {
                var User = await _userRepository.GetUserByEmail(ResetPasswordReqModel.Email);
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The User cannot validate to reset password";
                    return Result;
                }
                if (User.Status != UserStatus.RESETPASSWORD)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "The request is denied!";
                    return Result;
                }
                var HashedPasswordModel = Encoder.CreateHashPassword(ResetPasswordReqModel.Password);
                User.Password = HashedPasswordModel.HashedPassword;
                User.Salt = HashedPasswordModel.Salt;
                User.Status = UserStatus.ACTIVE;
                _ = await _userRepository.Update(User);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Reset password successfully!";
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> VerifyEmail(EmailVerificationReqModel verificationModel)
        {
            try
            {
                var user = await _userRepository.GetUserByVerificationToken(verificationModel.OTP);
                if (user != null && user.Otpexpiration > DateTime.Now)
                {

                    user.Status = UserStatus.ACTIVE;
                    await _userRepository.Update(user);

                    return new ResultModel
                    {
                        IsSuccess = true,
                        Code = 200,
                        Message = "Email verification successful."
                    };
                }
                else if (user.Otpexpiration < DateTime.Now)
                {

                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Expired verification otp.(10 minutes)"
                    };
                }
                else
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Wrong verification otp."
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 500,
                    Message = "An error occurred while processing your request."
                };
            }
        }

        public async Task<ResultModel> ResendVerifyOTP(UserResendOTPReqModel RegisterForm)
        {
            ResultModel Result = new();
            try
            {
                var existingUser = await _userRepository.GetUserByEmail(RegisterForm.Email);
                if (existingUser == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "User not found.";
                    return Result;
                }
                else
                {
                    string OTP = GenerateOTP();
                    DateTime expirationTime = DateTime.Now.AddMinutes(10);

                    string FilePath = "../BussinessObject/TemplateEmail/FirstInformation.html";
                    string Html = File.ReadAllText(FilePath);
                    Html = Html.Replace("{{Email}}", RegisterForm.Email);
                    Html = Html.Replace("{{OTP}}", $"{OTP}");

                    bool emailSent = await EmailUltilities.SendEmail(RegisterForm.Email, "Email Verification", Html);

                    if (emailSent)
                    {
                        existingUser.Otp = OTP;
                        existingUser.Otpexpiration = expirationTime;


                        await _userRepository.Update(existingUser);

                        Result.IsSuccess = true;
                        Result.Code = 200;
                        Result.Message = "Verification email sent successfully!";
                    }
                    else
                    {
                        // Handle email sending failure
                        Result.IsSuccess = false;
                        Result.Code = 500;
                        Result.Message = "Failed to send verification email. Please try again later.";
                    }
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