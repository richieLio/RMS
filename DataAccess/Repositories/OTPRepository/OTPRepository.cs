using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.OTPRepo
{
    public class OTPRepository : Repository<Otpverify>, IOTPRepository
    {
        private readonly HouseManagementContext _context;
        public OTPRepository( HouseManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Otpverify> GetOTPByUserId(Guid UserId)
        {
            return await _context.Otpverifies.Where(x => x.UserId.Equals(UserId)).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
        }
    }
}
