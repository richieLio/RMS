using DataAccess.Models.BillModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.BillServices
{
    public interface IBillServices
    {
        Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel);
    }
}
