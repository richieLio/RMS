using DataAccess.Models.BillModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.BillServices
{
    public interface IBillServices
    {
        Task<ResultModel> CreateBill(Guid userId ,BillCreateReqModel billCreateReqModel);
    }
}
