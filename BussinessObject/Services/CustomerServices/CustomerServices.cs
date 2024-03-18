using AutoMapper;
using DataAccess.Entities;
using DataAccess.Models.CustomerModel;
using DataAccess.Models.RoomModel;
using DataAccess.Repositories.CustomerRepository;
using DataAccess.Repositories.HouseRepository;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using Encoder = Business.Utilities.Encoder;


namespace BussinessObject.Services.CustomerServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly ICustomerRepository _customerRepository;

        private readonly IRoomRepository _roomRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly IUserRepository _userRepository;
        public CustomerServices(ICustomerRepository customerRepository, IHouseRepository houseRepository, IRoomRepository roomRepository, IUserRepository userRepository)
        {
            _customerRepository = customerRepository;
            _houseRepository = houseRepository;
            _roomRepository = roomRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultModel> GetCustomerProfile(Guid userId, Guid customerId)
        {
            ResultModel Result = new();
            try
            {
                var user = await _userRepository.Get(customerId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }


                var userProfile = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Address,
                    user.Gender,
                    user.Dob,
                    user.LicensePlates,
                    user.CitizenIdNumber,
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

        public async Task<ResultModel> Login(CustomerLoginReqModel LoginForm)
        {
            ResultModel Result = new();
            try
            {
                var account = await _customerRepository.GetHouseByAccountName(LoginForm.HouseAccount);
                if (account == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }
                else
                {
                    var Salt = account.Salt;
                    var PasswordStored = account.Password;
                    var Verify = Encoder.VerifyPasswordHashed(LoginForm.Password, Salt, PasswordStored);
                    if (Verify)
                    {

                        CustomerLoginResModel LoginResData = new CustomerLoginResModel();

                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<House, HouseResModel>();
                        });
                        IMapper mapper = config.CreateMapper();
                        HouseResModel UserResModel = mapper.Map<House, HouseResModel>(account);

                        LoginResData.User = UserResModel;
                        LoginResData.Token = Encoder.GenerateHouseJWT(account);
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

        public async Task<ResultModel> UpdateUserProfile(Guid userId, CustomerUpdateModel customerUpdateModel)
        {
            ResultModel Result = new();
            try
            {
                var user = await _userRepository.Get(userId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }
                var userToUpdate = await _userRepository.Get(customerUpdateModel.Id);

                if (userToUpdate == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Customer not found";
                    return Result;
                }



                userToUpdate.Email = customerUpdateModel.Email;
                userToUpdate.PhoneNumber = customerUpdateModel.PhoneNumber;
                userToUpdate.Address = customerUpdateModel.Address;
                userToUpdate.Gender = customerUpdateModel.Gender;
                userToUpdate.Dob = customerUpdateModel.Dob;
                userToUpdate.FullName = customerUpdateModel.FullName;
                userToUpdate.LicensePlates = customerUpdateModel.LicensePlates;
                userToUpdate.Status = customerUpdateModel.Status;

                _ = await _userRepository.Update(user);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = user;
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

        public async Task<ResultModel> VerifySecondPass(string token, SecondPassVerificationReqModel secondPassVerificationModel)
        {
            ResultModel Result = new();
            try
            {
                Encoder encoder = new Encoder();
                Guid houseId = new Guid(encoder.DecodeToken(token, "houseid"));

                var house = await _houseRepository.Get(houseId);

                var room = await _roomRepository.Get(secondPassVerificationModel.RoomId);

                if (house == null || room == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "Not found";
                }
                else
                {


                    var PasswordStored = room.SecondPassword;

                    var Verify = Encoder.Verify2ndPasswordHashed(secondPassVerificationModel.SecondPassword, PasswordStored);
                    if (Verify)
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<Room, RoomResModel>();
                        });

                        IMapper mapper = config.CreateMapper();
                        RoomResModel roomResModel = mapper.Map<RoomResModel>(room);



                        Result.IsSuccess = true;
                        Result.Code = 200;
                        Result.Message = "Password is valid";
                        Result.Data = roomResModel;
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
    }
}
