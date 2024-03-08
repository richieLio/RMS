using BussinessObject.Ultilities;
using DataAccess.Entities;
using DataAccess.Models.ContractModel;
using DataAccess.Models.RoomModel;
using DataAccess.Repositories.ContractRepository;
using DataAccess.Repositories.CustomerRepository;
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
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoomRepository _roomRepository;

        public ContractServices(IContractRepository contractRepository, IUserRepository userRepository, ICustomerRepository customerRepository, IRoomRepository roomRepository)
        {
            _contractRepository = contractRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _roomRepository = roomRepository;
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
                        StartDate = c.StartDate.HasValue == true ? c.StartDate.Value.ToString("dd/MM/yyyy") : null,
                        EndDate = c.EndDate.HasValue == true ? c.EndDate.Value.ToString("dd/MM/yyyy") : null,
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

        public async Task<ResultModel> GetContractInformation(Guid contractId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var Contracts = await _contractRepository.GetContractById(contractId);
                var OwnerContractDetails = await _userRepository.GetUserByID(Contracts.OwnerId);
                var Customers = await _customerRepository.GetCustomerByUserId(Contracts.CustomerId);
                var Rooms = await _roomRepository.GetRoomById((Guid)Contracts.RoomId);

                if (Contracts == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Contract not found";
                    return result;
                }

                OwnerContractDetails OwnerContractBy = new()
                {
                    Id = OwnerContractDetails.Id,
                    Name = OwnerContractDetails.FullName
                };

                CustomerContractDetails customerContractDetails = new()
                {
                    Id = Customers.Id,
                    Email = Customers.Email,
                    PhoneNumber = Customers.PhoneNumber,
                    Address = Customers.Address,
                    Gender = Customers.Gender,
                    Dob = Customers.Dob.HasValue == true ? Customers.Dob.Value.ToString("dd/MM/yyyy") : null,
                    FullName = Customers.FullName,
                    LicensePlates = Customers.LicensePlates,
                    CreatedAt = Customers.CreatedAt.HasValue == true ? Customers.CreatedAt.Value.ToString("dd/MM/yyyy") : null,
                    CitizenIdNumber = Customers.CitizenIdNumber,
                };

                RoomContractDetails roomContractDetails = new()
                {
                    Id = Rooms.Id,
                    Name = Rooms.Name,
                };

                var contractInformation = new ContractInfoResModel
                {
                    Id = Contracts.Id,
                    Owner = OwnerContractBy,
                    CustomerDetails = customerContractDetails,
                    RoomDetails = roomContractDetails,
                    StartDate = Contracts.StartDate.HasValue == true ? Contracts.StartDate.Value.ToString("dd/MM/yyyy") : null,
                    EndDate = Contracts.EndDate.HasValue == true ? Contracts.EndDate.Value.ToString("dd/MM/yyyy") : null,
                    ImagesUrl = Contracts.ImagesUrl,
                    FileUrl = Contracts.FileUrl,
                    Status = Contracts.Status,
                };

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = contractInformation;
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
