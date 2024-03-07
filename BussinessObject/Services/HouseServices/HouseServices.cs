using AutoMapper;
using BussinessObject.Ultilities;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.HouseModel;
using DataAccess.Repositories.HouseRepository;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encoder = Business.Ultilities.Encoder;
using EmailUltilities = Business.Ultilities.Email;
using Data.Enums;
using DataAccess.Repositories.UserRepository;


namespace BussinessObject.Services.HouseServices
{
    public class HouseServices : IHouseServices
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IUserRepository _userRepository;
        public HouseServices(IHouseRepository houseRepository, IUserRepository userRepository)
        {
            _houseRepository = houseRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel houseCreateReqModel)
        {
            ResultModel Result = new();
            try
            {
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


                var availableRoom = houseCreateReqModel?.AvailableRoom;
                var roomQuantity = houseCreateReqModel?.RoomQuantity;

                NewHouse.Id = Guid.NewGuid();
                NewHouse.OwnerId = ownerId;
                NewHouse.Password = HashedPasswordModel.HashedPassword;
                NewHouse.Salt = HashedPasswordModel.Salt;
                NewHouse.RoomQuantity = roomQuantity;
                NewHouse.AvailableRoom = availableRoom;
                NewHouse.Status = GeneralStatus.ACTIVE;
                if(availableRoom > roomQuantity){
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Available room must be smaller than room quantity";
                    return Result;
                }
                _ = await _houseRepository.Insert(NewHouse);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Create House successfully!";
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

        public async Task<ResultModel> GetHousesByUserId(Guid UserId, int page)
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
                house.RoomQuantity = houseUpdateReqModel.RoomQuantity;
                house.AvailableRoom = houseUpdateReqModel.AvailableRoom;
                _ = await _houseRepository.Update(house);
                result.IsSuccess = true;
                result.Code = 200;
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
