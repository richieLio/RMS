using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.VerifyServices
{
    public interface IVerifyServices
    {
        public Task<ResultModel> SendOTPEmailRequest(string Email);

        public Task<ResultModel> VerifyOTPCode(string Email, string OTPCode);
    }
}
