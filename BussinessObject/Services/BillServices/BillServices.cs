using AutoMapper;
using BussinessObject.Utilities;
using DataAccess.Entities;
using DataAccess.Models.BillModel;
using DataAccess.Models.ServiceModel;
using DataAccess.Repositories.BillRepository;
using DataAccess.Repositories.HouseRepository;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.ServiceFeeRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using Google.Api.Gax;
using MySqlX.XDevAPI.Common;

namespace BussinessObject.Services.BillServices
{
    public class BillServices : IBillServices
    {
        private readonly IBillRepository _billRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly IServiceFeeRepository _serviceFeeRepository;
        public BillServices(IBillRepository billRepository, 
            IUserRepository userRepository, IRoomRepository roomRepository,
            IHouseRepository houseRepository, IServiceFeeRepository serviceFeeRepository)
        {
            _billRepository = billRepository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
            _houseRepository = houseRepository;
            _serviceFeeRepository = serviceFeeRepository;
        }
        public async Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.Get(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<BillCreateReqModel, Bill>();
                });
                IMapper mapper = config.CreateMapper();
                Bill newBill = mapper.Map<BillCreateReqModel, Bill>(billCreateReqModel);

                // Thêm bill 
                newBill.Id = Guid.NewGuid();
                newBill.Month = DateTime.Now;
                newBill.IsPaid = false;
                newBill.CreateBy = userId;
                newBill.RoomId = billCreateReqModel?.RoomId;
                await _billRepository.Insert(newBill);


                var serviceQuantities = billCreateReqModel.ServiceQuantities;

                var isAddService = await _billRepository.AddServicesToBill(newBill.Id, serviceQuantities);



                if (!isAddService)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "Failed to add service to bill."
                    };
                }


                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Bill created successfully!"
                };


            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
        }

        public async Task<ResultModel> GetAllBills(Guid userId, int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
                    return result;
                }
                if (page == null || page == 0)
                {
                    page = 1;
                }
                var bills = await _billRepository.GetBillsByUserId(userId);
                List<BillResModel> billList = new List<BillResModel>();

                foreach (var bill in bills)
                {
                    var room = await _roomRepository.GetRoomById(bill.RoomId);
                    var house = await _houseRepository.Get(room.HouseId.Value);
                    string houseName = house.Name;
                    string roomName = room.Name;
                    BillResModel bl = new BillResModel()
                    {
                        Id = bill.Id,
                        TotalPrice = bill.TotalPrice,
                        Month = bill.Month,
                        IsPaid = bill.IsPaid,
                        PaymentDate = bill.PaymentDate,
                        CreateBy = bill.CreateBy,
                        RoomId = bill.RoomId,
                        RoomName = roomName,
                        HouseName = houseName
                    };
                    billList.Add(bl); // Add the created bill model to the list
                }

                var paginatedResult = await Pagination.GetPagination(billList, page, 10);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = paginatedResult;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultModel> getBillDetails(Guid userId, Guid billId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
                    return result;
                }

                var bills = await _billRepository.GetBillDetails(userId, billId);
                if (bills == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Bill not found";
                    return result;
                }
               


                var billServices = await _billRepository.GetBillServicesForBill(billId);
                if (billServices == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Bill services not found";
                    return result;
                }
                // take room and house name
                var bill = await _billRepository.Get(billId);

                var room = await _roomRepository.Get(bill.RoomId.Value);
                string roomName = room.Name;
                var house = await _houseRepository.Get(room.HouseId.Value);
                string houseName = house.Name;

                BillDetailsResModel billDetails = new BillDetailsResModel();
                billDetails.Id = billId;
                billDetails.TotalPrice = bills.TotalPrice;
                billDetails.Month = bills.Month;
                billDetails.IsPaid = bills.IsPaid;
                billDetails.PaymentDate = bills.PaymentDate;
                billDetails.RoomName = roomName;
                billDetails.HouseName = houseName;
                




                if (billServices != null)
                {
                    billDetails.Services = billServices.Select(bs => new BillServiceDetails
                    {
                        ServiceName = bs.Service?.Name, 
                        Quantity = bs.Quantity
                    }).ToList();
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = billDetails;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.Message = ex.Message;
                return result;
            }
        }


    }




}
