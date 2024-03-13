using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.OTPRepo
{
    public interface IOTPRepository : IRepository<Otpverify>
    {
        public Task<Otpverify> GetOTPByUserId(Guid UserId);
    }
}
