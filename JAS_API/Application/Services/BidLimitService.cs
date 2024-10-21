using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.BidLimitDTOs;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entity;
using Domain.Enums;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace Application.Services
{
    public class BidLimitService : IBidLimitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_FileBidLimit";

        public BidLimitService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<APIResponseModel> CreateNewBidLimit(CreateBidLimitDTO createBidLimitDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var uploadResult = await _cloudinary.UploadAsync(new RawUploadParams
                {
                    File = new FileDescription(createBidLimitDTO.File.FileName,
                               createBidLimitDTO.File.OpenReadStream()),
                    Tags = Tags,
                    Type = "upload"
                }).ConfigureAwait(false);

                if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    reponse.Message = $"File upload failed." + uploadResult.Error.Message + "";
                    reponse.Code = (int)uploadResult.StatusCode;
                    reponse.IsSuccess = false;
                }
                else
                {
                    var enity = _mapper.Map<BidLimit>(createBidLimitDTO);
                    enity.File = uploadResult.SecureUrl.AbsoluteUri;
                    enity.ExpireDate = DateTime.Now.AddMonths(6);
                    enity.Status = EnumStatusBidLimit.Pending.ToString();
                    enity.Reason = null;
                     await _unitOfWork.BidLimitRepository.AddAsync(enity);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.Message = $"File upload Successfull";
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        reponse.Data = _mapper.Map<BidLimitDTO>(enity);
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.ErrorMessages = ex.Message.Split(',').ToList();
                reponse.Message = "Exception";
                reponse.Code = 500;
                reponse.IsSuccess = false;
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewListBidLimit()
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimits = await _unitOfWork.BidLimitRepository.GetAllAsync(includes: x => x.Customer);
                var DTOs = new List<BidLimitDTO>();
                foreach (var bidLimit in bidLimits)
                {
                    BidLimitDTO mapper;
                    if (bidLimit.StaffId != null)
                    {
                        var staff = _unitOfWork.StaffRepository.GetByIdAsync(bidLimit.StaffId).Result;
                        string staffName = staff.FirstName + " " + staff.LastName;
                         mapper = _mapper.Map<BidLimitDTO>(bidLimit, x => x.Items["StaffName"] = staffName);
                        mapper.CustomerName = bidLimit.Customer.FirstName + " " + bidLimit.Customer.LastName;
                        DTOs.Add(mapper);
                    }
                     mapper = _mapper.Map<BidLimitDTO>(bidLimit, x => x.Items["StaffName"] = null);
                     mapper.CustomerName = bidLimit.Customer.FirstName + " " + bidLimit.Customer.LastName;
                     DTOs.Add(mapper);

                }
                if (DTOs.Count > 0)
                {
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Message = "Received List BidLimit Successfull";
                    reponse.Data = DTOs;
                }
                else
                {
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Message = "Received List Is Empty";
                }
                
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewBidLimitByCustomer(int customerId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimits = await _unitOfWork.BidLimitRepository.GetAllAsync(condition: x => x.CustomerId == customerId, includes: x => x.Customer);
                var DTOs = new List<BidLimitDTO>();
                foreach (var bidLimit in bidLimits)
                {
                    var staff = _unitOfWork.StaffRepository.GetByIdAsync(bidLimit.StaffId).Result;
                    string staffName = staff.FirstName + " " + staff.LastName;
                    var mapper = _mapper.Map<BidLimitDTO>(bidLimit, x => x.Items["StaffName"] = staffName);
                    mapper.CustomerName = bidLimit.Customer.FirstName + " " + bidLimit.Customer.LastName;
                    DTOs.Add(mapper);
                }
                if (DTOs.Count > 0)
                {
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Message = "Received List BidLimit Successfull";
                    reponse.Data = DTOs;
                }
                else
                {
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Message = "Received List BidLimit is Empty";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> UpdateStatus(UpdateBidLimitDTO updateBidLimitDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimit = await _unitOfWork.BidLimitRepository.GetByIdAsync(updateBidLimitDTO.Id, includes: x => x.Customer);
                if (bidLimit == null)
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found BidLimit";
                }
                else
                {
                    var status = EnumHelper.GetEnums<EnumStatusBidLimit>().FirstOrDefault(x => x.Value == updateBidLimitDTO.Status).Name;
                    if(status != null)
                    {
                        var check = (ValueTuple<bool, string>)checkStatus(bidLimit, status);
                        if (check.Item1 == true)
                        {
                            if(status == EnumStatusBidLimit.Processing.ToString())
                            {
                                bidLimit.PriceLimit = updateBidLimitDTO.PriceLimit;
                                bidLimit.StaffId = updateBidLimitDTO.StaffId;
                            }
                            if(status == EnumStatusBidLimit.Reject.ToString())
                            {
                                bidLimit.Reason = updateBidLimitDTO.Reason;
                            }
                            if(status == EnumStatusBidLimit.Approve.ToString())
                            {
                                bidLimit.Customer.PriceLimit = bidLimit.PriceLimit;
                                bidLimit.Customer.ExpireDate = bidLimit.ExpireDate;
                            }
                            bidLimit.Status = status;
                            _unitOfWork.BidLimitRepository.Update(bidLimit);
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                reponse.Code = 200;
                                reponse.IsSuccess = true;
                                reponse.Message = $"Update Status : {status} Successfull";
                            }
                            else
                            {
                                reponse.IsSuccess = false;
                                reponse.Message = $"Errer when saving.";
                            }
                        }
                        else
                        {
                            reponse.IsSuccess = false;
                            reponse.Message = $"Faild with status : {check.Item2.ToString()}";

                        }
                        
                    }
                    else
                    {
                        reponse.Code = 400;
                        reponse.IsSuccess = false;
                        reponse.Message = "Not Found Status BidLimit";
                    }
                    
                }
            }
            catch (Exception e)
            {
                reponse.Code = 400;
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        internal object checkStatus(BidLimit bidLimit, string status)
        {
            object result = (status: true, msg: "Status is suiable");
            switch (status)
            {
                case nameof(EnumStatusBidLimit.Pending):
                    if (bidLimit.Status == EnumStatusBidLimit.Pending.ToString()) 
                    {
                        result = (status: false, msg: "Status of object is pending , cannt set pending again");
                    }
                    if(bidLimit.Status == EnumStatusBidLimit.Approve.ToString() || bidLimit.Status == EnumStatusBidLimit.Reject.ToString())
                    {
                        result = (status: false, msg: "Status of object is Approve or Reject , cannt set Pending ");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Processing.ToString())
                    {
                        result = (status: false, msg: "Status of object is SetPrice , cannt set pending again");
                    }
                    break;
                case nameof(EnumStatusBidLimit.Processing):
                    //if (bidLimit.Status == EnumStatusBidLimit.Pending.ToString())
                    //{
                    //    result = (status: false, msg: "Status of object is setprrice , cannt set pending");
                    //}
                    //if (bidLimit.Status == EnumStatusBidLimit.SetPrice.ToString())
                    //{
                    //    result = (status: false, msg: "Status of object is setprrice , cannt set setprice again");
                    //}
                    if (bidLimit.Status == EnumStatusBidLimit.Approve.ToString() || bidLimit.Status == EnumStatusBidLimit.Reject.ToString())
                    {
                        result = (status: false, msg: "Status of object is Approve or Reject , cannt set SetPrice ");
                    }
                    break;
                case nameof(EnumStatusBidLimit.Approve):
                    if (bidLimit.Status == EnumStatusBidLimit.Pending.ToString())
                    {
                        result = (status: false, msg: "Status of object is Pending , cannt set Approve you must setprice before approve");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Approve.ToString())
                    {
                        result = (status: false, msg: "Status of object is Approve , cannt set Approve again");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Reject.ToString())
                    {
                        result = (status: false, msg: "Status of object is Reject , cannt set Approve");
                    }
                    break;
                case nameof(EnumStatusBidLimit.Reject):
                    if (bidLimit.Status == EnumStatusBidLimit.Pending.ToString())
                    {
                        result = (status: false, msg: "Status of object is Pending , cannt set Reject you must setprice before Reject");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Reject.ToString())
                    {
                        result = (status: false, msg: "Status of object is Reject , cannt set Reject again");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Approve.ToString())
                    {
                        result = (status: false, msg: "Status of object is Approve , cannt set Reject");
                    }
                    break;
                case nameof(EnumStatusBidLimit.Cancel):
                    if (bidLimit.Status == EnumStatusBidLimit.Cancel.ToString())
                    {
                        result = (status: false, msg: "Status of object is Cancel , cannt set Cancel again");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Approve.ToString())
                    {
                        result = (status: false, msg: "Status of object is Approve , cannt set Cancel");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Reject.ToString())
                    {
                        result = (status: false, msg: "Status of object is Reject , cannt set Cancel");
                    }
                    if (bidLimit.Status == EnumStatusBidLimit.Processing.ToString())
                    {
                        result = (status: false, msg: "Status of object is Processing , cannt set Cancel");
                    }
                    break;
            }
            return result;
        }
        public async Task<APIResponseModel> GetStatusBidLimt()
        {
            var reponse = new APIResponseModel();
            try
            {
                var filters = EnumHelper.GetEnums<EnumStatusBidLimit>();
                if (filters == null)
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Receive Filter Bid Limit Fail";
                    reponse.Code = 400;
                }
                else
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Receive Filter Bid Limit Successfull";
                    reponse.Code = 200;
                    reponse.Data = filters;
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = true;
                reponse.Message = e.Message;
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewBidLimitById(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var bidLimit = await _unitOfWork.BidLimitRepository.GetByIdAsync(Id, includes: x => x.Customer);
                if (bidLimit != null)
                {
                    var staff = _unitOfWork.StaffRepository.GetByIdAsync(bidLimit.StaffId).Result;
                    string staffName = staff.FirstName + " " + staff.LastName;
                    var mapper = _mapper.Map<BidLimitDTO>(bidLimit, x => x.Items["StaffName"] = staffName);
                    mapper.CustomerName = bidLimit.Customer.FirstName + " " + bidLimit.Customer.LastName;
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Message = "Received List BidLimit Successfull";
                    reponse.Data = mapper;
                }
                else
                {
                    reponse.Code = 404;
                    reponse.IsSuccess = false;
                    reponse.Message = "Received List BidLimit Faild";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> FilterBidLimtByStatus(int status)
        {
            var reponse = new APIResponseModel();
            try
            {
                var enumStatus = EnumHelper.GetEnums<EnumStatusBidLimit>().FirstOrDefault(x => x.Value == status)?.Name;

                if (enumStatus != null) 
                {
                    var bidLimits = await _unitOfWork.BidLimitRepository.GetAllAsync(
                        includes: x => x.Customer,
                        condition: x => x.Status == enumStatus
                    );
                        var DTOs = new List<BidLimitDTO>();
                        foreach (var bidLimit in bidLimits)
                        {
                            var staff = _unitOfWork.StaffRepository.GetByIdAsync(bidLimit.StaffId).Result;
                            string staffName = staff.FirstName + " " + staff.LastName;
                            var mapper = _mapper.Map<BidLimitDTO>(bidLimit, x => x.Items["StaffName"] = staffName);
                            mapper.CustomerName = bidLimit.Customer.FirstName + " " + bidLimit.Customer.LastName;
                            DTOs.Add(mapper);
                        }
                        if (DTOs.Count > 0)
                        {
                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                            reponse.Message = $"Received List BidLimit Successfull by status {EnumHelper.GetEnums<EnumStatusBidLimit>().FirstOrDefault(x => x.Value == status).Name}";
                            reponse.Data = DTOs;
                        }
                        else
                        {
                            reponse.Code = 200;
                            reponse.IsSuccess = true;
                            reponse.Message = $"Not Found BidLimit Have This Status {enumStatus}";
                        }

                }
                else
                {
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Status In System";
                }
            }
            catch (Exception e)
            {
                reponse.IsSuccess = false;
                reponse.Message = "Exception";
                reponse.ErrorMessages = new List<string> { e.Message };
            }
            return reponse;
        }

    }
}
