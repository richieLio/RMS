using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.UserRepository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly HouseManagementContext _context;
        public UserRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> CheckIfCustomerIsExisted(string Email, string phoneNumber, string CitizenIdNumber, string LicensePlates)
        {
            return await _context.Users
                .Where(x => x.Email.Equals(Email) || x.PhoneNumber.Equals(phoneNumber)
                            || x.CitizenIdNumber.Equals(CitizenIdNumber) || x.LicensePlates.Equals(LicensePlates)).FirstOrDefaultAsync();

           
        }

        public async Task<User> GetUserByEmail(string Email)
        {
            return await _context.Users.Where(x => x.Email.Equals(Email)).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByID(Guid? userId)
        {
            return await _context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        }

       
    }
}
