using DataAccess.Models.CustomerModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.CustomerServices
{
    public interface ICustomerServices
    {
        Task<ResultModel> UpdateUserProfile(Guid userId, CustomerUpdateModel customerUpdateModel);
        Task<ResultModel> GetCustomerProfile(Guid userId, Guid customerId);
    }
}
