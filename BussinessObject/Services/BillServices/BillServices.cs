using AutoMapper;
using Data.Enums;
using DataAccess.Entities;
using DataAccess.Models.BillModel;
using DataAccess.Models.HouseModel;
using DataAccess.Repositories.BillRepository;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.BillServices
{
    public class BillServices : IBillServices
    {
        private readonly IBillRepository _billRepository;

        public BillServices(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }
        public async Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel)
        {
            ResultModel Result = new();
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<BillCreateReqModel, Bill>();
                });
                IMapper mapper = config.CreateMapper();
                Bill newBill = mapper.Map<BillCreateReqModel, Bill>(billCreateReqModel);

                //giá tiền phòng
                var rentAmount = billCreateReqModel.RentAmount;
                // giá tiền điện
                var electricUnitPrice = billCreateReqModel.ElectricityUnitPrice;
                // số điện đã sử dụng
                var electricUsed = billCreateReqModel.ElectricityUsed;
                // giá tiền nước 
                var waterUnitPrice = billCreateReqModel.WaterUnitPrice;
                //số nước đã sử dụng
                var waterUsed = billCreateReqModel.WaterUsed;
                // phí dịch vụ
                var servicePrice = billCreateReqModel.ServicePrice;

               

                newBill.Id = Guid.NewGuid();
                newBill.RentAmount = rentAmount;
                newBill.ElectricityUnitPrice = electricUnitPrice;
                newBill.ElectricityUsed = electricUsed;
                newBill.WaterUnitPrice = waterUnitPrice;
                newBill.WaterUsed = waterUsed;
                newBill.ServicePrice = billCreateReqModel.ServicePrice;
                newBill.TotalPice = rentAmount + (electricUnitPrice * (decimal)electricUsed) + (waterUnitPrice * (decimal)waterUsed) + servicePrice;
                newBill.Month = DateTime.Now;
                newBill.IsPaid = false;
                newBill.CreateBy = userId;
                newBill.RoomId = billCreateReqModel?.RoomId;
                _ = await _billRepository.Insert(newBill);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Message = "Bill created successfully!";
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
