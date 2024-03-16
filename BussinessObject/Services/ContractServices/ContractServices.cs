using BussinessObject.Utilities;
using DataAccess.Entities;
using DataAccess.Models.ContractModel;
using DataAccess.Repositories.ContractRepository;
using DataAccess.Repositories.CustomerRepository;
using DataAccess.Repositories.RoomRepository;
using DataAccess.Repositories.UserRepository;
using DataAccess.ResultModel;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.Contracts;

namespace BussinessObject.Services.ContractServices
{
    public class ContractServices : IContractServices
    {
        private readonly IContractRepository _contractRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly CloudStorage _cloudStorage;

        public ContractServices(CloudStorage cloudStorage,IContractRepository contractRepository, IUserRepository userRepository, ICustomerRepository customerRepository, IRoomRepository roomRepository)
        {
            _contractRepository = contractRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _roomRepository = roomRepository;
            _cloudStorage = cloudStorage;
        }

        public async Task<ResultModel> GetContractList(Guid userId, int page)
        {
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.Get(userId); // Await here
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

                var contracts = await _contractRepository.GetContractsByOwnerId(userId);
                List<ContractInfoResModel> contractList = new List<ContractInfoResModel>();

                foreach (var contract in contracts)
                {
                    var owner = await _userRepository.Get(contract.OwnerId.Value); // Await here
                    var customer = await _customerRepository.GetCustomerByUserId(contract.CustomerId); // Await here
                    var room = await _roomRepository.Get(contract.RoomId.Value); // Await here

                    OwnerContractDetails ownerContractBy = new OwnerContractDetails
                    {
                        Id = owner.Id,
                        Name = owner.FullName
                    };

                    CustomerContractDetails customerContractDetails = new CustomerContractDetails
                    {
                        Id = customer.Id,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        Address = customer.Address,
                        Gender = customer.Gender,
                        Dob = customer.Dob?.ToString("dd/MM/yyyy"),
                        FullName = customer.FullName,
                        LicensePlates = customer.LicensePlates,
                        CreatedAt = customer.CreatedAt?.ToString("dd/MM/yyyy"),
                        CitizenIdNumber = customer.CitizenIdNumber,
                    };

                    RoomContractDetails roomContractDetails = new RoomContractDetails
                    {
                        Id = room.Id,
                        Name = room.Name,
                    };

                    ContractInfoResModel contractInfo = new ContractInfoResModel
                    {
                        Id = contract.Id,
                        Owner = ownerContractBy,
                        CustomerDetails = customerContractDetails,
                        RoomDetails = roomContractDetails,
                        StartDate = contract.StartDate?.ToString("dd/MM/yyyy"),
                        EndDate = contract.EndDate?.ToString("dd/MM/yyyy"),
                        ImagesUrl = contract.ImagesUrl,
                        FileUrl = contract.FileUrl,
                        Status = contract.Status,
                    };

                    contractList.Add(contractInfo);
                }

                var paginatedResult = await Pagination.GetPagination(contractList, page, 10);

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = paginatedResult;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }



        public async Task<ResultModel> GetContractInformation(Guid userId, Guid contractId)
        {
            ResultModel result = new ResultModel();
            try
            {

                var user = _userRepository.Get(userId); 
                var Contracts = await _contractRepository.GetContractById(userId, contractId);
                var OwnerContractDetails = await _userRepository.GetUserByID(Contracts.OwnerId);
                var Customers = await _customerRepository.GetCustomerByUserId(Contracts.CustomerId);
                var Rooms = await _roomRepository.GetRoomById((Guid)Contracts.RoomId);

                if (Contracts == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Contract not found";
                    return result;
                }
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
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

        public async Task<ResultModel> UpdateContract(Guid userId, ContractReqModel contractReqModel, string filePath)
        {
            ResultModel result = new();
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
                var Contract = await _contractRepository.Get(contractReqModel.Id);
                if (Contract == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Contract not found";
                    return result;
                }

                Contract.EndDate = contractReqModel.EndDate;
                Contract.ImagesUrl = filePath;
              

                _ = await _contractRepository.UpdateByOwnerId(userId, Contract);
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = Contract;
                result.Message = "Contract updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> UpdateContractStatus(Guid userId, ContractUpdateStatusReqModel contractUpdateStatusReqModel)
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
                var Contract = await _contractRepository.GetContractById(userId, contractUpdateStatusReqModel.Id);
                if (Contract == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Contract not found";
                    return result;
                }

                Contract.Status = contractUpdateStatusReqModel.Status;

                _ = await _contractRepository.Update(Contract);
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Contract updated status successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultModel> GetContractByRoom(Guid userId, Guid roomId)
        {
            ResultModel result = new ResultModel();

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
                var Contracts = await _contractRepository.GetContractByRoomId(userId, roomId);
                List<ContractOfRoomModel> contractOfRoomList = new();
                foreach (var c in Contracts)
                {
                    var Owners = await _userRepository.GetUserByID(c.OwnerId);
                    var Customers = await _customerRepository.GetCustomerByUserId(c.CustomerId);

                    OwnerDetailModel OwnerBy = new()
                    {
                        Email = Owners.Email,
                        FullName = Owners.FullName,
                        PhoneNumber = Owners.PhoneNumber,
                    };

                    CustomerDetailModel CustomerBy = new()
                    {
                        Email = Customers.Email,
                        FullName = Customers.FullName,
                        PhoneNumber = Customers.PhoneNumber,
                    };

                    ContractOfRoomModel cr = new()
                    {
                        Owner = OwnerBy,
                        Customer = CustomerBy,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        ImagesUrl = c.ImagesUrl,
                        FileUrl = c.FileUrl,
                        Status = c.Status,
                    };
                    contractOfRoomList.Add(cr);
                }
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = contractOfRoomList;
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
