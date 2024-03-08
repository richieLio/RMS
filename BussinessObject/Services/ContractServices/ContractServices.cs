using BussinessObject.Ultilities;
using DataAccess.Models.ContractModel;
using DataAccess.Models.RoomModel;
using DataAccess.Repositories.ContractRepository;
using DataAccess.Repositories.HouseRepository;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.ContractServices
{
    public class ContractServices : IContractServices
    {
        private readonly IContractRepository _contractRepository;

        public ContractServices(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<ResultModel> GetContractList(int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (page == null || page == 0)
                {
                    page = 1;
                }

                var Contracts = await _contractRepository.GetContracts();
                List<ContractResModel> contractList = new();
                foreach (var c in Contracts)
                {

                    ContractResModel cl = new()
                    {
                        Id = c.Id,
                        OwnerId = c.OwnerId,
                        CustomerId = c.CustomerId,
                        RoomId = c.RoomId,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        ImagesUrl = c.ImagesUrl,
                        FileUrl = c.FileUrl,
                        Status = c.Status,
                    };
                    contractList.Add(cl);

                }
                var ResultList = await Pagination.GetPagination(contractList, page, 10);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = ResultList;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
