using AutoMapper;
using BussinessObject.Utilities;
using Data.Enums;
using DataAccess.Entities;
using DataAccess.Models.RoomModel;
using DataAccess.Models.ServiceModel;
using DataAccess.Repositories.ServiceFeeRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Services.ServiceFeeServices
{
    public class ServiceFeeServices : IServiceFeeServices
    {
        private readonly IServiceFeeRepository _serviceFeeRepository;
        private readonly IUserRepository _userRepository;


        public ServiceFeeServices(IServiceFeeRepository serviceRepository, IUserRepository userRepository)
        {
            _serviceFeeRepository = serviceRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultModel> AddNewService(Guid userId, ServiceCreateReqModel serviceReqModel)
        {

            ResultModel Result = new();
            try
            {


                var user = await _userRepository.Get(userId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "User not found.";
                    return Result;
                }


                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ServiceCreateReqModel, Service>();
                });
                IMapper mapper = config.CreateMapper();
                Service newService = mapper.Map<ServiceCreateReqModel, Service>(serviceReqModel);

                newService.Name = serviceReqModel.Name;
                newService.Price = serviceReqModel.Price;



                await _serviceFeeRepository.Insert(newService);
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = newService;
                Result.Message = "Create service successfully!";



            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }


        public async Task<ResultModel> GetServicesList(Guid userId, int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
                    return result;
                }
                if (page == null || page == 0)
                {
                    page = 1;
                }
                var services = await _serviceFeeRepository.GetAllServices();
                List<ServiceResModel> servicesList = new();
                foreach (var service in services)
                {
                    ServiceResModel sr = new()
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price,
                    };
                    servicesList.Add(sr);
                }


                var paginatedResult = await Pagination.GetPagination(servicesList, page, 10);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = paginatedResult;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 404;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultModel> RemoveService(Guid userId, Guid serviceId)
        {
            ResultModel result = new();
            try
            {
                var user = _userRepository.Get(userId);
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
                    return result;
                }
                var service = await _serviceFeeRepository.Get(serviceId);
                if (service == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Service Not found";
                    return result;
                }

                _ = await _serviceFeeRepository.Remove(service);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Services Removed successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel)
        {
            ResultModel result = new();
            try
            {
                var user = _userRepository.Get(userId);
                var service = await _serviceFeeRepository.Get(serviceUpdateModel.Id);
                if (service == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                service.Name = serviceUpdateModel.Name;
                service.Price = serviceUpdateModel.Price;
                _ = await _serviceFeeRepository.Update(service);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Services updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
