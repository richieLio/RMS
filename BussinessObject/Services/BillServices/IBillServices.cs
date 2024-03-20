using DataAccess.Models.BillModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.BillServices
{
    public interface IBillServices
    {
        Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel);
        Task<ResultModel> GetAllBills(Guid userId, int page);
        Task<ResultModel> getBillDetails(Guid userId, Guid billId);
        
    }
}
