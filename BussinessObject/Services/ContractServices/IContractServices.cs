using DataAccess.Models.ContractModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;

namespace BussinessObject.Services.ContractServices
{
    public interface IContractServices
    {
        Task<ResultModel> GetContractList(Guid userId, int page);
        Task<ResultModel> GetContractInformation(Guid userId, Guid contractId);
        Task<ResultModel> UpdateContract(Guid userId, ContractReqModel contractReqModel, string filePath);
        Task<ResultModel> UpdateContractStatus(Guid userId, ContractUpdateStatusReqModel contractUpdateStatusReqModel);
        Task<ResultModel> GetContractByRoom(Guid userId, Guid roomId);

        Task<(string filePath, string fileName)> DownloadFile(Guid userId, Guid contractId);


    }
}
