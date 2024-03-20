using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.UserRepository
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> GetUserByEmail(string Email);

        public Task<User> GetUserByID(Guid? userId);

        public Task<User> CheckIfCustomerIsExisted(Guid roomId, string Email, string phoneNumber, string CitizenIdNumber, string LicensePlates);

        Task<User> GetUserByVerificationToken(string otp);

    }
}
