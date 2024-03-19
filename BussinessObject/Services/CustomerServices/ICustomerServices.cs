using DataAccess.Models.CustomerModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.CustomerServices
{
    public interface ICustomerServices
    {
        public Task<ResultModel> Login(CustomerLoginReqModel LoginForm);

        Task<ResultModel> VerifySecondPass(string token, SecondPassVerificationReqModel secondPassVerificationModel);
        Task<ResultModel> UpdateUserProfile(Guid userId, CustomerUpdateModel customerUpdateModel);
        Task<ResultModel> GetCustomerProfile(Guid userId, Guid customerId);
    }
}
