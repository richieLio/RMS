using BussinessObject.Services.ContractServices;
using BussinessObject.Services.RoomServices;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/contract")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractServices _contractServices;

        public ContractController(IContractServices contractServices)
        {
            _contractServices = contractServices;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetContractList(int page)
        {
            ResultModel result = await _contractServices.GetContractList(page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
