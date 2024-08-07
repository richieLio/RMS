﻿using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.CustomerRepository
{
    public class CustomerRepository : Repository<House>, ICustomerRepository
    {
        private readonly HouseManagementContext _context;

        public CustomerRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
        }

       

        public async Task<User> GetCustomerByUserId(Guid? userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}
