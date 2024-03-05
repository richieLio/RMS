using BussinessObject.Services.HouseServices;
using BussinessObject.Services.RoomServices;
using DataAccess.Models.CustomerModel;
using DataAccess.Models.HouseModel;
using DataAccess.Models.RoomModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomServices _roomServices;

        public RoomController(IRoomServices roomServices)
        {
            _roomServices = roomServices;
        }

        [HttpPost("add-new")]
        public async Task<IActionResult> CreateHouse([FromBody] RoomCreateReqModel Form)
        {
            ResultModel result = await _roomServices.AddRangeRoom(Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add-customer-to-room")]
        public async Task<IActionResult> AddCusomerToRoom([FromBody] CustomerCreateReqModel Form)
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
            ResultModel result = await _roomServices.AddCustomerToRoom(userId, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("information")]
        public async Task<IActionResult> getHouseInfoById(Guid roomId)
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
            ResultModel result = await _roomServices.GetCustomerByRoomId(userId, roomId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
