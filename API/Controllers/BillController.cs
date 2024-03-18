using BussinessObject.Services.BillServices;
using DataAccess.Models.BillModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/bill")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillServices _billServices;

        public BillController(IBillServices billServices)
        {
            _billServices = billServices;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllBill(int page)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            ResultModel result = await _billServices.GetAllBills(userId, page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    

    [HttpPost("add-new")]
        public async Task<IActionResult> CreateBill([FromBody] BillCreateReqModel Form)
        {
            var userIdString = User.FindFirst("userid")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest("Unable to retrieve user ID");
            }
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Invalid user ID format");
            }
            ResultModel result = await _billServices.CreateBill(userId, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
