using AutoMapper;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.EmailModel;
using DataAccess.Models.UserModel;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using EmailUltilities = Business.Utilities.Email;
using Encoder = Business.Utilities.Encoder;

namespace BussinessObject.Services.UserServices
{
    public class UserSevices : IUserSevices
    {

        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;
        public UserSevices(IUserRepository userRepository, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _httpClient = httpClient;
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
                user.Dob = updateModel.Dob;


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

        public async Task<ResultModel> CreateOrLoginWithFacebook(string accessToken)
        {
            ResultModel result = new ResultModel();

            try
            {
                // Make request to Facebook's Graph API using the access token
                var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields=email,name,picture.type(large)&access_token={accessToken}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<FacebookUserData>(content);

                var user = await _userRepository.GetUserByEmail(userData.Email);

                if (user != null)
                {
                    // If user already exists, prepare login response
                    UserLoginResModel loginResData = PrepareLoginResponse(user);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = loginResData;
                    result.Message = "Login successful!";
                }
                else
                {
                    // If user does not exist, create a new account
                    var newUser = CreateUserFromFacebookData(userData);
                    await _userRepository.Insert(newUser);

                    // Prepare login response for the new user
                    UserLoginResModel loginResData = PrepareLoginResponse(newUser);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = loginResData;
                    result.Message = "Account created and logged in successfully!";
                }
            }
            catch (HttpRequestException ex)
            {
                HandleException(result, ex);
            }
            catch (Exception e)
            {
                HandleException(result, e);
            }

            return result;
        }

        private User CreateUserFromFacebookData(FacebookUserData userData)
        {
            return new User
            {
                Email = userData.Email,
                FullName = userData.Name.Length > 50 ? userData.Name.Substring(0, 50) : userData.Name,
                Role = UserEnum.OWNER,
                Status = UserStatus.ACTIVE,
                CreatedAt = DateTime.Now,
                Address = "N/A",
                Gender = "N/A",
                PhoneNumber = "N/A"
            };
        }

        private UserLoginResModel PrepareLoginResponse(User user)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserResModel>();
            });
            IMapper mapper = config.CreateMapper();
            UserResModel userResModel = mapper.Map<User, UserResModel>(user);

            return new UserLoginResModel
            {
                User = userResModel,
                Token = Encoder.GenerateJWT(user)
            };
        }

        private void HandleException(ResultModel result, Exception ex)
        {
            result.IsSuccess = false;
            result.Code = ex is HttpRequestException ? (int)((HttpRequestException)ex).StatusCode : 500;
            result.Message = "An error occurred while creating the account.";
            result.ResponseFailed = ex.Message;
        }

        public async Task<ResultModel> CreateOrLoginWithGoogle(string credential)
        {
            ResultModel result = new ResultModel();

            try
            {
                // Decode and verify JWT token
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(credential) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    throw new Exception("Invalid JWT token");
                }

                // Extract user information from the JWT token
                var email = jsonToken.Payload["email"].ToString();

                var user = await _userRepository.GetUserByEmail(email);

                if (user != null)
                {
                    // If user already exists, prepare login response
                    UserLoginResModel loginResData = PrepareLoginResponse(user);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = loginResData;
                    result.Message = "Login successful!";
                }
                else
                {
                    // If user does not exist, create a new account
                    var newUser = CreateUserFromGoogleData(jsonToken.Payload);
                    await _userRepository.Insert(newUser);

                    // Prepare login response for the new user
                    UserLoginResModel loginResData = PrepareLoginResponse(newUser);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = loginResData;
                    result.Message = "Account created and logged in successfully!";
                }
            }
            catch (HttpRequestException ex)
            {
                HandleException(result, ex);
            }
            catch (Exception e)
            {
                HandleException(result, e);
            }

            return result;
        }
        private User CreateUserFromGoogleData(JwtPayload payload)
        {
            var email = payload["email"].ToString();
            var fullname = payload["name"].ToString();
            return new User
            {
                Email = email,
                FullName = fullname, 
                Role = UserEnum.OWNER,
                Status = UserStatus.ACTIVE,
                CreatedAt = DateTime.Now,
                Address = "N/A",
                Gender = "N/A",
                PhoneNumber = "N/A"
            };
        }
    }
}
