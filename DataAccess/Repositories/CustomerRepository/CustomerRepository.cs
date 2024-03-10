using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.CustomerRepository
{
    public class CustomerRepository : Repository<House>, ICustomerRepository
    {
        private readonly HouseManagementContext _context;

        public CustomerRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

        public async Task<House> GetHouseByAccountName(string name)
        {
            return await _context.Houses.Where(x => x.HouseAccount.Equals(name)).FirstOrDefaultAsync();
        }

        public async Task<User> GetCustomerByUserId(Guid? userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}
