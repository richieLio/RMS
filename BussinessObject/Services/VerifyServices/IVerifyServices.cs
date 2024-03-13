
using DataAccess.Models.UserModel;
using DataAccess.Models.VerifyModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAccess.ResultModel;


namespace BussinessObject.Services.VerifyServices
{
    public interface IVerifyServices
    {
        public Task<ResultModel> SendOTPEmailRequest(SendOTPReqModel sendOTPReqModel);

        public Task<ResultModel> VerifyOTPCode(string email, string otpCode);
    }
}
