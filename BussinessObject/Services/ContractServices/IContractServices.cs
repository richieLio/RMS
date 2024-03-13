using DataAccess.Models.ContractModel;
using DataAccess.ResultModel;

namespace BussinessObject.Services.ContractServices
{
    public interface IContractServices
    {
        Task<ResultModel> GetContractList(int page);
        Task<ResultModel> GetContractInformation(Guid contractId);
        Task<ResultModel> UpdateContract(ContractReqModel contractReqModel);
        Task<ResultModel> UpdateContractStatus(ContractUpdateStatusReqModel contractUpdateStatusReqModel);
    }
}
