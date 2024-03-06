using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.UserRepository
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetUserByEmail(string Email);

        public Task<User> GetUserByID(Guid? userId);

        public Task<User> CheckIfCustomerIsExisted(string Email, string phoneNumber, string CitizenIdNumber, string LicensePlates);

        Task<User> GetUserByVerificationToken(string verificationToken);

    }
}
