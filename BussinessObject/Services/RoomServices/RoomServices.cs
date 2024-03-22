using AutoMapper;
using BussinessObject.Utilities;
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
using MySqlX.XDevAPI.Common;
using EmailUltilities = Business.Utilities.Email;
using Encoder = Business.Utilities.Encoder;



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

        public async Task<ResultModel> AddRangeRoom(Guid userId, RoomCreateRangeReqModel roomCreateReqModel)
        {
            ResultModel Result = new();
            try
            {
                int? UpdateRoomQuantity = await _houseRepository.GetRoomQuantityByHouseId(roomCreateReqModel.HouseId);
                var house = await _houseRepository.Get(roomCreateReqModel.HouseId);
                var user = await _userRepository.GetUserByID(userId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "User not found.";
                    return Result;
                }
                if (house == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House not found";
                    return Result;
                }

                int? RoomQuantity = roomCreateReqModel.Quantity;
                if (RoomQuantity.HasValue && RoomQuantity > 0)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<RoomCreateRangeReqModel, Room>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var newRooms = new List<Room>();
                    for (int i = 0; i < RoomQuantity.Value; i++)
                    {
                        Room newRoom = mapper.Map<RoomCreateRangeReqModel, Room>(roomCreateReqModel);

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

                    if (UpdateRoomQuantity.HasValue)
                    {
                        int newRoomQuantity = UpdateRoomQuantity.Value + RoomQuantity.Value;
                        house.RoomQuantity = newRoomQuantity;
                        house.AvailableRoom += RoomQuantity.Value;
                        await _houseRepository.Update(house);
                    }
                }
                else
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House not found";
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
        public async Task<ResultModel> AddRoom(Guid userId, RoomCreateReqModel roomCreateReqModel)
        {
            ResultModel Result = new();
            try
            {
                int? UpdateRoomQuantity = await _houseRepository.GetRoomQuantityByHouseId(roomCreateReqModel.HouseId);
                var house = await _houseRepository.Get(roomCreateReqModel.HouseId);
                var user = await _userRepository.GetUserByID(userId);
                var existingRoom = await _roomRepository.GetRoomByName(roomCreateReqModel.HouseId, roomCreateReqModel.Name);
                if (existingRoom != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Room with this name already exists.";
                    return Result;
                }
                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "User not found.";
                    return Result;
                }
                if (house == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House not found";
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RoomCreateReqModel, Room>();
                });
                IMapper mapper = config.CreateMapper();
                Room newRoom = mapper.Map<RoomCreateReqModel, Room>(roomCreateReqModel);

                newRoom.Id = Guid.NewGuid();
                newRoom.HouseId = roomCreateReqModel.HouseId;
                newRoom.Name = roomCreateReqModel.Name;
                newRoom.Status = RoomStatus.EMPTY;

                await _roomRepository.Insert(newRoom);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = newRoom;
                Result.Message = "Create rooms successfully!";

                if (UpdateRoomQuantity.HasValue)
                {
                    house.RoomQuantity += 1;
                    house.AvailableRoom += 1;
                    await _houseRepository.Update(house);
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

        public async Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel)
        {
            ResultModel result = new();
            try
            {
                var customerCreateReqModel = addCustomerToRoomReqModel.customerCreateReqModel;
                var houseUpdateAvaiableRoom = addCustomerToRoomReqModel.houseUpdateAvaiableRoom;
                var room = await _roomRepository.Get(customerCreateReqModel.RoomId);

                var user = await _userRepository.GetUserByID(userId);
                var house = await _houseRepository.Get(houseUpdateAvaiableRoom.HouseId);
                int availableRoom = await _houseRepository.GetAvailableRoomByHouseId(houseUpdateAvaiableRoom.HouseId);

                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found.";
                    return result;
                }
                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "House not found.";
                    return result;
                }
                var check = await _userRepository.CheckIfCustomerIsExisted(customerCreateReqModel.RoomId, customerCreateReqModel.Email, customerCreateReqModel.PhoneNumber,
                    customerCreateReqModel.CitizenIdNumber, customerCreateReqModel.LicensePlates);
                if (check != null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User is existed";
                    return result;
                }
                else
                {
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
                        EndDate = customerCreateReqModel.EndDate,
                        Status = GeneralStatus.ACTIVE,
                    };
                    await _contractRepository.Insert(contract);

                    // sửa số phòng còn trống
                    if (room.Status == RoomStatus.EMPTY)
                    {
                        house.AvailableRoom = availableRoom - 1;
                        _ = await _houseRepository.Update(house);
                    }

                    //gửi mail mật khẩu cấp 2 cho khách

                   
                    string FilePath = "../BussinessObject/TemplateEmail/Create2ndPassword.html";

                    string Html = File.ReadAllText(FilePath);
                   
                    Html = Html.Replace("{{RoomName}}", $"{room.Name}");
                    //  Html = Html.Replace("{{HousePassword}}", $"{house.Password}");
                    bool emailSent = await EmailUltilities.SendEmail(customerCreateReqModel.Email, "Email Notification", Html);

                    if (!emailSent)
                    {
                        // Xử lý trường hợp gửi email không thành công
                        result.IsSuccess = false;
                        result.Code = 500;
                        result.Message = "Email cannot be send.";
                        return result;
                    }
                    //update status
                    room.Status = RoomStatus.ENTIRE;
                    _ = await _roomRepository.Update(room);


                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Customer added to room successfully.";
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
     
        public async Task<ResultModel> GetCustomerByRoomId(int page, Guid userId, Guid roomId)
        {
            ResultModel result = new ResultModel();

            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }
                var user = _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found.";
                    return result;
                }
                var customers = await _roomRepository.GetCustomersByRoomId(roomId);

                var customerModels = customers.Select(c => new CustomerResModel
                {

                    Id = c.Id,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Gender = c.Gender,
                    Dob = c.Dob.HasValue == true ? c.Dob.Value.ToString("dd/MM/yyyy") : null,
                    FullName = c.FullName,
                    LicensePlates = c.LicensePlates,
                    CreatedAt = c.CreatedAt.HasValue == true ? c.CreatedAt.Value.ToString("dd/MM/yyy") : null,
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
                    var ResultList = await Pagination.GetPagination(customers, page, 10);
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = ResultList;
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

        public async Task<ResultModel> GetRoomList(int page, Guid userId, Guid houseId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = _userRepository.Get(userId);
                var house = _houseRepository.Get(houseId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found.";
                    return result;
                }
                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "House not found.";
                    return result;
                }
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var rooms = await _roomRepository.GetRooms(houseId);
                List<RoomResModel> roomList = new();
                foreach (var r in rooms)
                {

                    RoomResModel rl = new()
                    {
                        Id = r.Id,
                        Name = r.Name,
                        HouseId = r.HouseId,
                        Status = r.Status
                    };
                    roomList.Add(rl);

                }
                var ResultList = await Pagination.GetPagination(roomList, page, 10);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = ResultList;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetRoomInformation(Guid roomId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var Room = await _roomRepository.GetRoomById(roomId);
                var House = await _houseRepository.GetHouseById(Room.HouseId);

                var OwnerHouseDetails = await _userRepository.GetUserByID(House.OwnerId);

                if (Room == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Room not found";
                    return result;
                }

                OwnerHouseDetailsModel OwnerHouseBy = new()
                {
                    Id = OwnerHouseDetails.Id,
                    Name = OwnerHouseDetails.FullName
                };

                HouseDetails houseDetails = new()
                {
                    Id = House.Id,
                    Owner = OwnerHouseBy,
                    Name = House.Name,
                    Address = House.Address,
                    RoomQuantity = House.RoomQuantity,
                    AvailableRoom = House.AvailableRoom,
                    Status = House.Status,
                };

                var roomInformation = new RoomInfoResModel
                {
                    Id = Room.Id,
                    Name = Room.Name,
                    houseDetails = houseDetails,
                    Status = House.Status,
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = roomInformation;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel)
        {
            ResultModel result = new();
            try
            {
                var Room = await _roomRepository.GetRoomById(roomUpdateReqModel.Id);
                if (Room == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                Room.Name = roomUpdateReqModel.Name;
                _ = await _roomRepository.Update(Room);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Room updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> UpdateRoomStatus(RoomUpdateStatusReqModel roomUpdateStatusReqModel)
        {
            ResultModel result = new();
            try
            {
                var Room = await _roomRepository.GetRoomById(roomUpdateStatusReqModel.Id);
                if (Room == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Room Not found";
                    return result;
                }
                Room.Status = roomUpdateStatusReqModel.Status;
                _ = await _roomRepository.Update(Room);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Room updated status successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<ResultModel> GetRoomRevenueForPeriod(Guid userId,Guid houseId, DateTime startDate, DateTime endDate)
        {
            ResultModel result = new ResultModel();
            try
            {

                var roomRevenueData = await _roomRepository.GetRoomRevenueForPeriod(userId,houseId, startDate, endDate);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = roomRevenueData;
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
