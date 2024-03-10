using AutoMapper;
using Data.Enums;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.CustomerModel;
using DataAccess.Models.HouseModel;
using DataAccess.Models.RoomModel;
using DataAccess.Repositories.ContractRepository;
using DataAccess.Repositories.HouseRepository;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.RoomServices
{
    public class RoomServices : IRoomServices
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IContractRepository _contractRepository;
        public RoomServices(IRoomRepository roomRepository, IHouseRepository houseRepository, IUserRepository userRepository, IContractRepository contractRepository)
        {
            _roomRepository = roomRepository;
            _houseRepository = houseRepository;
            _userRepository = userRepository;
            _contractRepository = contractRepository;
        }

        public async Task<ResultModel> AddRangeRoom(RoomCreateReqModel roomCreateReqModel)
        {
            ResultModel Result = new();
            try
            {
                int? RoomQuantity = await _houseRepository.GetRoomQuantityByHouseId(roomCreateReqModel.HouseId);
                if (RoomQuantity.HasValue && RoomQuantity > 0)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RoomCreateReqModel, Room>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var newRooms = new List<Room>();
                    for (int i = 0; i < RoomQuantity.Value; i++) 
                    {
                        Room newRoom = mapper.Map<RoomCreateReqModel, Room>(roomCreateReqModel);

                        newRoom.Id = Guid.NewGuid();
                        newRoom.HouseId = roomCreateReqModel.HouseId;
                        newRoom.Name = roomCreateReqModel.Name + i;
                        newRoom.Status = GeneralStatus.ACTIVE;
                        newRooms.Add(newRoom);
                    }
                    await _roomRepository.AddRange(newRooms);
                    Result.IsSuccess = true;
                    Result.Code = 200;
                    Result.Message = "Create rooms successfully!";
                }
                else
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House not found or no rooms available.";
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
        
        public async Task<ResultModel> AddCustomerToRoom(Guid userId, CustomerCreateReqModel customerCreateReqModel, HouseUpdateAvaiableRoomReqModel houseUpdateAvaiableRoom)
        {
            ResultModel result = new();
            try
            {
                var user = await _userRepository.GetUserByID(userId);
                var house = await _houseRepository.Get(houseUpdateAvaiableRoom.HouseId);
                int availableRoom = await _houseRepository.GetAvailableRoomByHouseId(houseUpdateAvaiableRoom.HouseId);

                if (user == null || house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found.";
                    return result;
                }
                var check = await _userRepository.CheckIfCustomerIsExisted(customerCreateReqModel.Email, customerCreateReqModel.PhoneNumber,
                    customerCreateReqModel.CitizenIdNumber, customerCreateReqModel.LicensePlates);
                if (check != null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User is existed";
                    return result;
                }
                // thêm thông tin khách

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = customerCreateReqModel.Email,
                    PhoneNumber = customerCreateReqModel.PhoneNumber,
                    Address = customerCreateReqModel.Address,
                    Gender = customerCreateReqModel.Gender,
                    Dob = customerCreateReqModel.Dob,
                    FullName = customerCreateReqModel.FullName,
                    LicensePlates = customerCreateReqModel.LicensePlates,
                    Status = GeneralStatus.ACTIVE,
                    CreatedAt = DateTime.Now,
                    Role = UserEnum.CUSTOMER,
                    CitizenIdNumber = customerCreateReqModel.CitizenIdNumber
                };


                await _userRepository.Insert(newUser);

                // thêm khách vào phòng
                var isAddedToRoom = await _roomRepository.AddUserToRoom(newUser.Id, customerCreateReqModel.RoomId);
                if (!isAddedToRoom)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Failed to add customer to room. Room not found or user already exists in the room.";
                    return result;
                }

                // thêm hợp đồng
                var contract = new Contract
                {
                    Id = Guid.NewGuid(),
                    OwnerId = userId,
                    CustomerId = newUser.Id,
                    RoomId = customerCreateReqModel.RoomId,
                    StartDate = DateTime.Now,
                    EndDate = customerCreateReqModel.EndDate 
                };
                await _contractRepository.Insert(contract);

                // sửa số phòng còn trống
                house.AvailableRoom = availableRoom - 1;
                _ = await _houseRepository.Update(house);


                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Customer added to room successfully.";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetCustomerByRoomId(Guid userId, Guid roomId)
        {
            ResultModel result = new ResultModel();

            try
            {
                var customers = await _roomRepository.GetCustomersByRoomId(roomId);

                var customerModels = customers.Select(c => new CustomerResModel
                {
                    
                    Id = c.Id,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Gender  = c.Gender,
                    Dob = c.Dob,
                    FullName = c.FullName,
                    LicensePlates = c.LicensePlates,
                    CreatedAt = c.CreatedAt,
                    CitizenIdNumber = c.CitizenIdNumber,
                    // Map other properties accordingly
                }).ToList();
                if (customers == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "There is no customer in this room";
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = customerModels;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
