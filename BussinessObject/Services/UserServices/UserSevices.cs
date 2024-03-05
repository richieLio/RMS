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
using System.Threading.Tasks;
using DataAccess.Enums;
using Business.Ultilities;

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
                if (User == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
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
                    Result.Message = "Email is existed!";
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore()); ;
                    });
                    IMapper mapper = config.CreateMapper();
                    User NewUser = mapper.Map<UserReqModel, User>(RegisterForm);

                    var HashedPasswordModel = Encoder.CreateHashPassword(RegisterForm.Password);
                    NewUser.Id = Guid.NewGuid();
                    NewUser.Password = HashedPasswordModel.HashedPassword;
                    NewUser.Salt = HashedPasswordModel.Salt;
                    NewUser.Status = UserStatus.ACTIVE;
                    NewUser.CreatedAt = DateTime.Now;
                    NewUser.Role = UserEnum.OWNER;
                    _ = await _userRepository.Insert(NewUser);
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Message = "Create account successfully!";
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


    }
}
