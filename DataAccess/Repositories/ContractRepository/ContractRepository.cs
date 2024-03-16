using Data.Enums;
using DataAccess.Entities;
using DataAccess.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.ContractRepository
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        private readonly HouseManagementContext _context;
        private readonly DbSet<Contract> _contracts;

        public ContractRepository(HouseManagementContext context) : base(context)
        {
            _context = context;
            _contracts = context.Set<Contract>();
        }

        public async Task<IEnumerable<Contract>> GetContractsByOwnerId(Guid onwerId)
        {
            return await _contracts.Where(x => x.Status.Equals(GeneralStatus.ACTIVE) && x.OwnerId == onwerId).ToListAsync();
        }

        public async Task<Contract?> GetContractById(Guid ownerId, Guid contractId)
        {
            return await _contracts
                .FirstOrDefaultAsync(c => c.Id == contractId && c.OwnerId == ownerId);
        }

        public async Task<IEnumerable<Contract>> GetContractByRoomId(Guid ownerId, Guid roomId)
        {
            return await _context.Contracts.Where(r => r.RoomId == roomId && r.OwnerId == ownerId).ToListAsync();
        }

        public async Task<Contract> UpdateByOwnerId(Guid ownerId, Contract contract)
        {
            return  = await _context.Contracts.FirstOrDefaultAsync(c => c.OwnerId == ownerId && c.Id == contract.Id);
        }
    }
}
