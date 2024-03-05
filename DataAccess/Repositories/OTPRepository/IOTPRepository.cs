using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.OTPRepo
{
    public interface IOTPRepository : IRepository<Otpverify>
    {
        public Task<Otpverify> GetOTPByUserId(Guid UserId);
    }
}
