using AutoMapper;
using BussinessObject.Ultilities;
using Data.Enums;
using DataAccess.Entities;
using DataAccess.Models.HouseModel;
using DataAccess.Models.RoomModel;
using DataAccess.Repositories.HouseRepository;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using MySqlX.XDevAPI.Common;
using Encoder = Business.Ultilities.Encoder;


namespace BussinessObject.Services.HouseServices
{
    public class HouseServices : IHouseServices
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;
        public HouseServices(IHouseRepository houseRepository, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _houseRepository = houseRepository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel houseCreateReqModel)
        {
            ResultModel Result = new();
            try
            {
                var existingHouse = await _houseRepository.GetHouseByName(houseCreateReqModel.Name);
                if (existingHouse != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House with this name already exists.";
                    return Result;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<HouseCreateReqModel, House>().ForMember(dest => dest.Password, opt => opt.Ignore()); ;
                });
                IMapper mapper = config.CreateMapper();
                House NewHouse = mapper.Map<HouseCreateReqModel, House>(houseCreateReqModel);
                if (houseCreateReqModel.Password == null)
                {
                    houseCreateReqModel.Password = Encoder.GenerateRandomPassword();
                }

                var HashedPasswordModel = Encoder.CreateHashPassword(houseCreateReqModel.Password);

                NewHouse.OwnerId = ownerId;
                NewHouse.Password = HashedPasswordModel.HashedPassword;
                NewHouse.Salt = HashedPasswordModel.Salt;
                NewHouse.RoomQuantity = 0;
                NewHouse.AvailableRoom = 0;
                NewHouse.Status = GeneralStatus.ACTIVE;
               
                _ = await _houseRepository.Insert(NewHouse);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = NewHouse;
                Result.Message = "House created successfully";
            }


            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> GetHouseById(Guid userId, Guid houseId)
        {
            ResultModel result = new ResultModel();

            try
            {
                var house = await _houseRepository.Get(houseId);

                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "House not found";
                    return result;
                }

                var Owner = await _userRepository.GetUserByID(house.OwnerId);
                OwnerHouseInfo OwnerBy = new()
                {
                    Id = Owner.Id,
                    Name = Owner.FullName,
                };

                var houseInfomation = new HouseInfoResModel();
                {
                    houseInfomation.Id = house.Id;
                    houseInfomation.OwnerId = OwnerBy;
                    houseInfomation.Name = house.Name;
                    houseInfomation.Address = house.Address;
                    houseInfomation.RoomQuantity = house.RoomQuantity;
                    houseInfomation.AvailableRoom = house.AvailableRoom;
                    houseInfomation.Status = house.Status;
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = houseInfomation;


            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetHousesByUserId(int page, Guid UserId)
        {
            ResultModel result = new ResultModel();

            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }
                var houses = await _houseRepository.GetAllHouseByUserId(UserId);

                List<HouseListResModel> housesList = new();

                foreach (var house in houses)
                {
                    var Owner = await _userRepository.GetUserByID(house.OwnerId);
                    OwnerHouseModel OwnerBy = new()
                    {
                        Id = Owner.Id,
                        Name = Owner.FullName,
                    };
                    HouseListResModel houseListResModel = new()
                    {
                        Id = house.Id,
                        OwnerId = OwnerBy,
                        Name = house.Name,
                        Address = house.Address,
                        RoomQuantity = house.RoomQuantity,
                        AvailableRoom = house.AvailableRoom,
                    };
                    housesList.Add(houseListResModel);
                }
                var ResultList = await Pagination.GetPagination(housesList, page, 10);
                return new ResultModel { IsSuccess = true, Data = ResultList };
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateAvailableRoom(Guid userId, HouseUpdateAvaiableRoomReqModel houseUpdateAvaiableRoom)
        {

            ResultModel Result = new();
            try
            {
                var user = await _userRepository.Get(userId);
                var house = await _houseRepository.Get(houseUpdateAvaiableRoom.HouseId);
                int availableRoom = await _houseRepository.GetAvailableRoomByHouseId(houseUpdateAvaiableRoom.HouseId);
                if (user == null || house == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }

                house.AvailableRoom = houseUpdateAvaiableRoom.AvailableRoom;


                _ = await _houseRepository.Update(house);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Available room updated successfully";
            }
            catch (Exception ex)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.Message = "User doesn't have the required roles";

            }
            return Result;

        }

        public async Task<ResultModel> UpdateHouse(Guid OwnerId, HouseUpdateReqModel houseUpdateReqModel)
        {
            ResultModel result = new();
            try
            {
                var owner = await _userRepository.GetUserByID(OwnerId);
                if (owner == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                var house = await _houseRepository.GetHouseById(houseUpdateReqModel.Id);
                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                house.OwnerId = OwnerId;
                house.Name = houseUpdateReqModel.Name;
                house.Address = houseUpdateReqModel.Address;
                _ = await _houseRepository.Update(house);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = house;
                result.Message = "House updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> UpdateHouseStatus(Guid ownerId, HouseUpdateStatusReqModel houseUpdateStatusReqModel)
        {
            ResultModel result = new();
            try
            {
                var owner = await _userRepository.GetUserByID(ownerId);
                if (owner == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                var house = await _houseRepository.GetHouseById(houseUpdateStatusReqModel.Id);
                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                house.Id = houseUpdateStatusReqModel.Id;
                house.Status = houseUpdateStatusReqModel.Status;
                _ = await _houseRepository.Update(house);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "House updated status successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;

        }
    }
}
