using AutoMapper;
using DataAccess.Entities;
using DataAccess.Models.BillModel;
using DataAccess.Repositories.BillRepository;
using DataAccess.Repositories.ServiceRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using MySqlX.XDevAPI.Common;

namespace BussinessObject.Services.BillServices
{
    public class BillServices : IBillServices
    {
        private readonly IBillRepository _billRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;
        public BillServices(IBillRepository billRepository, IServiceRepository serviceRepository, IUserRepository userRepository)
        {
            _billRepository = billRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
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


    }
}
