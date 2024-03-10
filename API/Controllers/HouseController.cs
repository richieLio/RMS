using BussinessObject.Services.HouseServices;
using DataAccess.Entities;
using DataAccess.Models.HouseModel;
using DataAccess.Models.RoomModel;
using DataAccess.Models.UserModel;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/house")]
    [ApiController]
    public class HouseController : ControllerBase
    {
        private readonly IHouseServices _houseServices;

        public HouseController(IHouseServices houseServices)
        {
            _houseServices = houseServices;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetHouses(int page)
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
            ResultModel result = await _houseServices.GetHousesByUserId(userId, page);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("add-new")]
        public async Task<IActionResult> CreateHouse([FromBody] HouseRoomCreateReqModel Form)
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
            ResultModel result = await _houseServices.AddHouse(userId, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] HouseUpdateReqModel Form)
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
            ResultModel result = await _houseServices.UpdateHouse(userId, Form);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
       
        [HttpGet("information")]
        public async Task<IActionResult> getHouseInfoById(Guid houseId)
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
            ResultModel result = await _houseServices.GetHouseById(userId, houseId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

