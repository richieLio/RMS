using DataAccess.Models.CustomerModel;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.CustomerServices
{
    public interface ICustomerServices
    {
        public Task<ResultModel> Login(CustomerLoginReqModel LoginForm);

        public Task<ResultModel> CreateSecondPass(string token, CustomerCreate2ndPassReqModel customerCreate2NdPassReqModel);
        Task<ResultModel> VerifySecondPass(string token, SecondPassVerificationReqModel secondPassVerificationModel);
    }
}
