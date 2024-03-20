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

        
    }
}
