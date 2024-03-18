using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;

namespace DataAccess.Repositories.ContractRepository
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<IEnumerable<Contract>> GetContractsByOwnerId(Guid ownerId);
        Task<Contract?> GetContractById(Guid ownerId,Guid contractId);
        Task<IEnumerable<Contract>> GetContractByRoomId(Guid ownerId, Guid roomId);
        Task<Contract> UpdateByOwnerId(Guid ownerId, Contract contract);
    }
}
