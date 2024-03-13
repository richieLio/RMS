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

        public async Task<IEnumerable<Contract>> GetContracts()
        {
            return await _contracts.Where(x => x.Status.Equals(GeneralStatus.ACTIVE)).ToListAsync();
        }

        public async Task<Contract?> GetContractById(Guid contractId)
        {
            return await _contracts
                .FirstOrDefaultAsync(c => c.Id == contractId);
        }
    }
}
