﻿using DataAccess.Entities;
using DataAccess.Models.HouseModel;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.HouseServices
{
    public interface IHouseServices
    {
        public Task<ResultModel> GetHousesByUserId(Guid UserId, int page);
        public Task<ResultModel> AddHouse(Guid ownerId ,HouseCreateReqModel houseCreateReqModel);
        public Task<ResultModel> UpdateHouse(Guid ownerId, HouseUpdateReqModel houseUpdateReqModel);
        public Task<ResultModel> UpdateHouseStatus(Guid ownerId, HouseUpdateStatusReqModel houseUpdateStatusReqModel);
        public Task<ResultModel> GetHouseById(Guid userId, Guid houseId);
    }
}