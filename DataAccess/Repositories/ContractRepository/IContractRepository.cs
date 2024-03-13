using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.ContractRepository
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<IEnumerable<Contract>> GetContracts();
        Task<Contract?> GetContractById(Guid contractId);
    }
}
