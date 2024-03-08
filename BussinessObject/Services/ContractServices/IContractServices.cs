using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.ContractServices
{
    public interface IContractServices
    {
        Task<ResultModel> GetContractList(int page);
        Task<ResultModel> GetContractInformation(Guid contractId);
    }
}
