using DataAccess.Entities;
using DataAccess.Models.ServiceModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.ServiceFeeServices
{
    public interface IServiceFeeServices
    {
        Task<ResultModel> AddNewService(Guid userId, ServiceCreateReqModel service);
        Task<ResultModel> GetServicesList(Guid userId, int page);
        Task<ResultModel> GetServicesList(Guid userId); // for creating bill
        Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel);
        Task<ResultModel> RemoveService(Guid userId, Guid serviceId);
    }
}
