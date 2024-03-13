using DataAccess.ResultModel;

namespace BussinessObject.Services.VerifyServices
{
    public interface IVerifyServices
    {
        public Task<ResultModel> SendOTPEmailRequest(string Email);

        public Task<ResultModel> VerifyOTPCode(string Email, string OTPCode);
    }
}
