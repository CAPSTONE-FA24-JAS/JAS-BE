using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.JewelryDTOs;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class JewelryService : IJewelryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string TagsImageJewelry = "Backend_ImageJewelry";
        private const string TagsImageMianDiamond = "ImageMianDiamond";
        private const string TagsImageSecondDiamond = "ImageSecondDiamond";
        private const string TagsDocumentMainDiamond = "DocumentMainDiamond";
        private const string TagsDocumentSecondDiamond = "DocumentSecondDiamond";
        private const string TagsImageMainShaphie = "ImageMainShaphie";
        private const string TagsImageSecondShaphie = "ImageSecondShaphie";
        private const string TagsDocumentMainShaphie = "DocumentMainShaphie";
        private const string TagsDocumentSecondShaphie = "DocumentSecondShaphie";
        private const string TagsAuthorized = "FilePDF_Authorized";
        private readonly IGeneratePDFService _generatePDFService;

        public JewelryService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary, IGeneratePDFService generatePDFService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _generatePDFService = generatePDFService;
        }

        private async Task<UploadResult> uploadImageOnCloudary(IFormFile file, string tag)
        {

            var uploadImage = await _cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams
            {
                File = new FileDescription(file.FileName,
                                           file.OpenReadStream()),
                Tags = tag

            }).ConfigureAwait(false);

            if (uploadImage == null || uploadImage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Image upload failed: {uploadImage.Error?.Message ?? "Unknown error"}");
            }
            return uploadImage;
        }

        private async void AddHistoryValuation(int id, string status)
        {
            var historyValuation = new HistoryValuation()
            {
                StatusName = status,
                ValuationId = id,
                CreationDate = DateTime.Now,
            };
            await _unitOfWork.HistoryValuationRepository.AddAsync(historyValuation);
        }

        public async Task<APIResponseModel> CreateJewelryAsync(CreateFinalValuationDTO jewelryDTO)
        {
            var response = new APIResponseModel();
            try
            {

                var jewelry = _mapper.Map<Jewelry>(jewelryDTO);
                if (jewelry == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                var status = EnumStatusValuation.FinalValuated.ToString();
                jewelry.CreationDate = DateTime.Now;
                var valuation = await _unitOfWork.ValuationRepository.GetByIdAsync(jewelryDTO.ValuationId);

                valuation.Status = status;              
                

                AddHistoryValuation(jewelryDTO.ValuationId, status);

                await _unitOfWork.JewelryRepository.AddAsync(jewelry);
                await _unitOfWork.SaveChangeAsync();
                jewelry.Status = EnumStatusJewelry.Watting.ToString();
                if (jewelryDTO.ImageJewelries != null && jewelryDTO.ImageJewelries.Any())
                {
                    foreach (var image in jewelryDTO.ImageJewelries)
                    {
                        var uploadImage = await uploadImageOnCloudary(image, TagsImageJewelry);
                        var imageJewelryDTO = new ImageJewelryDTO
                        {
                            ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                            ThumbnailImage = uploadImage.SecureUrl.AbsoluteUri,
                            Title = "Image of jewelty",
                            JewelryId = jewelry.Id,
                        };

                        var imageDTO = _mapper.Map<ImageJewelry>(imageJewelryDTO);
                        await _unitOfWork.ImageJewelryRepository.AddAsync(imageDTO);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }

                if (jewelryDTO.KeyCharacteristicDetails != null && jewelryDTO.KeyCharacteristicDetails.Any())
                {    
                    List<KeyCharacteristicDetail> keyCharacteristicDetails = new List<KeyCharacteristicDetail>();
                    foreach (var key in jewelryDTO.KeyCharacteristicDetails)
                    {
                        var keyDTO = _mapper.Map<KeyCharacteristicDetail>(key);
                        keyDTO.JewelryId = jewelry.Id;
                        keyCharacteristicDetails.Add(keyDTO);                       
                    }
                    await _unitOfWork.KeyCharacteristicsDetailRepository.AddRangeAsync(keyCharacteristicDetails);
                    await _unitOfWork.SaveChangeAsync();
                }

                if (jewelryDTO.MainDiamonds != null && jewelryDTO.MainDiamonds.Any())
                {
                    
                    foreach (var diamond in jewelryDTO.MainDiamonds)
                    {
                        await AddMainDiamondAsync(diamond, jewelry.Id);
                    }
                }

                if (jewelryDTO.SecondaryDiamonds != null && jewelryDTO.SecondaryDiamonds.Any())
                {
                    foreach (var diamond in jewelryDTO.SecondaryDiamonds)
                    {
                        await AddSecondDiamondAsync(diamond, jewelry.Id);
                    }
                }

                if (jewelryDTO.MainShaphies != null && jewelryDTO.MainShaphies.Any())
                {
                    foreach (var shaphie in jewelryDTO.MainShaphies)
                    {
                        await AddMainShaphieAsync(shaphie, jewelry.Id);
                    }
                }

                if (jewelryDTO.SecondaryShaphies != null && jewelryDTO.SecondaryShaphies.Any())
                {
                    foreach (var shaphie in jewelryDTO.SecondaryShaphies)
                    {
                        await AddSecondaryShaphieAsync(shaphie, jewelry.Id);
                    }
                }

                var notification = new Notification
                {
                    Title = $"Final valuation Id:  {jewelry.Id} has been create",
                    Description = $" Thẩm định đã tạo định giá cuối cho valuation Id : {jewelry.Id}",
                    Is_Read = false,
                    NotifiableId = jewelry.Id,  //valuationId
                    AccountId = jewelry.Valuation.Staff.AccountId,
                    CreationDate = DateTime.UtcNow
                };

                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangeAsync();

                var finalValuation = _mapper.Map<JewelryDTO>(jewelry);

                response.Message = "Create Jewelry Successfully";
                response.Code = 200;
                response.IsSuccess = true;
                response.Data = finalValuation;
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }


        private async Task AddMainDiamondAsync(CreateDiamondDTO diamondDTO, int jewelryId)
        {

            var diamond = _mapper.Map<MainDiamond>(diamondDTO);
            diamond.JewelryId = jewelryId;
            diamond.CreationDate = DateTime.Now;
            await _unitOfWork.MainDiamondRepository.AddAsync(diamond);
            await _unitOfWork.SaveChangeAsync();

            await AddImageandDocumentsMainDiamondAsync(diamondDTO.ImageDiamonds, diamondDTO.DocumentDiamonds, diamond.Id);
        }

        private async Task AddImageandDocumentsMainDiamondAsync(IEnumerable<IFormFile>? images, IEnumerable<IFormFile>? documents, int diamondId)
        {
            if (images != null)
            {
                List<ImageMainDiamond> imageList = new List<ImageMainDiamond>();
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageMianDiamond);

                    var imageMainDiamond = new ImageDiamondDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                        
                    };

                    var imageMainDiamondDTO = _mapper.Map<ImageMainDiamond>(imageMainDiamond);       
                    imageMainDiamondDTO.MainDiamondId = diamondId;
                    imageList.Add(imageMainDiamondDTO);        
                }

                await _unitOfWork.ImageMainDiamondRepository.AddRangeAsync(imageList);
                await _unitOfWork.SaveChangeAsync();
            }

            if (documents != null)
            {
                List<DocumentMainDiamond> documentList = new List<DocumentMainDiamond>();
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentMainDiamond);

                    var documentMainDiamond = new DocumentDiamondDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Diamond",
                        
                    };

                    var documentMainDiamondDTO = _mapper.Map<DocumentMainDiamond>(documentMainDiamond);
                    documentMainDiamondDTO.MainDiamondId = diamondId;
                    documentList.Add(documentMainDiamondDTO);
                   
                }
                await _unitOfWork.DocumentMainDiamondRepository.AddRangeAsync(documentList);
                await _unitOfWork.SaveChangeAsync();
            }


        }


        private async Task AddSecondDiamondAsync(CreateDiamondDTO diamondDTO, int jewelryId)
        {
            var diamond = _mapper.Map<SecondaryDiamond>(diamondDTO);
            diamond.JewelryId = jewelryId;
            diamond.CreationDate = DateTime.Now;
            await _unitOfWork.SecondDiamondRepository.AddAsync(diamond);
            await _unitOfWork.SaveChangeAsync();

            await AddImageandDocumentsSecondDiamondAsync(diamondDTO.ImageDiamonds, diamondDTO.DocumentDiamonds, diamond.Id);
        }
        private async Task AddImageandDocumentsSecondDiamondAsync(IEnumerable<IFormFile>? images, IEnumerable<IFormFile>? documents, int diamondId)
        {
            if (images != null)
            {
                List<ImageSecondaryDiamond> imageList = new List<ImageSecondaryDiamond>();
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageSecondDiamond);

                    var imageSecondDiamond = new ImageDiamondDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                       
                    };

                    var imageSecondDiamondDTO = _mapper.Map<ImageSecondaryDiamond>(imageSecondDiamond);
                    imageSecondDiamondDTO.SecondaryDiamondId = diamondId;
                    imageList.Add(imageSecondDiamondDTO);
                    
                }

                await _unitOfWork.ImageSecondDiamondRepository.AddRangeAsync(imageList);
                await _unitOfWork.SaveChangeAsync();
            }

            if (documents != null)
            {
                List<DocumentSecondaryDiamond> documentList = new List<DocumentSecondaryDiamond>();
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentSecondDiamond);

                    var documentSecondDiamond = new DocumentDiamondDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Diamond",
                       
                    };

                    var documentSecondDiamondDTO = _mapper.Map<DocumentSecondaryDiamond>(documentSecondDiamond);
                    documentSecondDiamondDTO.SecondaryDiamondId = diamondId;
                    documentList.Add(documentSecondDiamondDTO);

                    
                }
                await _unitOfWork.DocumentSecondaryDiamondRepository.AddRangeAsync(documentList);
                await _unitOfWork.SaveChangeAsync();
            }

        }


        private async Task AddMainShaphieAsync(CreateShaphieDTO shaphieDTO, int jewelryId)
        {
            var shaphie = _mapper.Map<MainShaphie>(shaphieDTO);
            shaphie.JewelryId = jewelryId;
            shaphie.CreationDate = DateTime.Now;
            await _unitOfWork.MainShaphieRepository.AddAsync(shaphie);
            await _unitOfWork.SaveChangeAsync();

            await AddImageandDocumentsMainShaphieAsync(shaphieDTO.ImageShaphies, shaphieDTO.DocumentShaphies, shaphie.Id);
        }

        private async Task AddImageandDocumentsMainShaphieAsync(IEnumerable<IFormFile>? images, IEnumerable<IFormFile>? documents, int shaphieId)
        {
            if (images != null)
            {
                List<ImageMainShaphie> imageList = new List<ImageMainShaphie>();
                foreach (var image in images)
                {

                    var uploadImage = await uploadImageOnCloudary(image, TagsImageMainShaphie);

                    var imageMainShaphie = new ImageShaphieDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                         
                    };

                    var imageMainShaphieDTO = _mapper.Map<ImageMainShaphie>(imageMainShaphie);
                    imageMainShaphieDTO.MainShaphieId = shaphieId;
                    imageList.Add(imageMainShaphieDTO);
                   
                }
                await _unitOfWork.ImageMainShaphieRepository.AddRangeAsync(imageList);
                await _unitOfWork.SaveChangeAsync();
            }

            if (documents != null)
            {
                List<DocumentMainShaphie> documentList = new List<DocumentMainShaphie>();
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentMainShaphie);

                    var documentMainShaphie = new DocumentShaphieDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Shaphie",
                        
                    };

                    var documentMainShaphieDTO = _mapper.Map<DocumentMainShaphie>(documentMainShaphie);
                    documentMainShaphieDTO.MainShaphieId = shaphieId;
                    documentList.Add(documentMainShaphieDTO);
                    
                }
                await _unitOfWork.DocumentMainShaphieRepository.AddRangeAsync(documentList);
                await _unitOfWork.SaveChangeAsync();
            }



        }

        private async Task AddSecondaryShaphieAsync(CreateShaphieDTO shaphieDTO, int jewelryId)
        {
            var shaphie = _mapper.Map<SecondaryShaphie>(shaphieDTO);
            shaphie.JewelryId = jewelryId;
            shaphie.CreationDate = DateTime.Now;
            await _unitOfWork.SecondaryShaphieRepository.AddAsync(shaphie);
            await _unitOfWork.SaveChangeAsync();

            await AddImageandDocumentsSecondaryShaphieAsync(shaphieDTO.ImageShaphies, shaphieDTO.DocumentShaphies, shaphie.Id);
        }

        private async Task AddImageandDocumentsSecondaryShaphieAsync(IEnumerable<IFormFile>? images, IEnumerable<IFormFile>? documents, int shaphieId)
        {
            if (images != null)
            {
                List<ImageSecondaryShaphie> imageList = new List<ImageSecondaryShaphie>();
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageSecondShaphie);

                    var imageSecondShaphie = new ImageShaphieDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                        
                    };

                    var imageSecondShaphieDTO = _mapper.Map<ImageSecondaryShaphie>(imageSecondShaphie);
                    imageSecondShaphieDTO.SecondaryShaphieId = shaphieId;
                    imageList.Add(imageSecondShaphieDTO);
                    
                }
                await _unitOfWork.ImageSecondaryShaphieRepository.AddRangeAsync(imageList);
                await _unitOfWork.SaveChangeAsync();
            }

            if (documents != null)
            {
                List<DocumentSecondaryShaphie> documentList = new List<DocumentSecondaryShaphie>();
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentSecondShaphie);

                    var documentSecondShaphie = new DocumentShaphieDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Shaphie",
                        
                    };

                    var documentSecondShaphieDTO = _mapper.Map<DocumentSecondaryShaphie>(documentSecondShaphie);
                    documentSecondShaphieDTO.SecondaryShaphieId = shaphieId;
                    documentList.Add(documentSecondShaphieDTO);
                    
                }
                await _unitOfWork.DocumentSecondaryShaphieRepository.AddRangeAsync(documentList);
                await _unitOfWork.SaveChangeAsync();
            }


        }

        public async Task<APIResponseModel> GetJewelryAsync(int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                var jewelrys = await _unitOfWork.JewelryRepository.GetAllPaging(filter: null,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Valuation,ImageJewelries",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);

            //    var jewelrys = await _unitOfWork.JewelryRepository.GetJewelrysAynsc(pageSize, pageIndex);
                List<JewelryListDTO> listjewelryDTO = new List<JewelryListDTO>();
                if (jewelrys.totalItems > 0)
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in jewelrys.data)
                    {
                        var jewelrysResponse = _mapper.Map<JewelryListDTO>(item);
                        listjewelryDTO.Add(jewelrysResponse);
                    };

                    var dataresponse = new
                    {
                        DataResponse = listjewelryDTO,
                        totalItemRepsone = jewelrys.totalItems
                    };

                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have valuations";
                    response.Code = 404;
                    response.IsSuccess = true;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> RequestFinalValuationForManagerAsync(RequestFinalValuationForManagerDTO requestDTO)
        {
            var response = new APIResponseModel();
            try
            {
                var valuationById = await _unitOfWork.JewelryRepository.GetByIdAsync(requestDTO.JewelryId);
                if (valuationById != null)
                {

                    valuationById.StartingPrice = requestDTO.StartingPrice;
                    valuationById.Time_Bidding = requestDTO.Time_Bidding;
                    valuationById.BidForm = EnumHelper.GetEnums<EnumLotType>().FirstOrDefault(x => x.Value == requestDTO.BidForm).Name; ;

                    _unitOfWork.JewelryRepository.Update(valuationById);
                    await _unitOfWork.SaveChangeAsync();

                    var valuationDTO = _mapper.Map<JewelryDTO>(valuationById);

                    response.Message = $"Update Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationDTO;
                }
                else
                {
                    response.Message = $"Not found valuation";
                    response.Code = 404;
                    response.IsSuccess = true;
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> UpdateStatusByManagerAsync(int jewelryId, int status)
        {
            var response = new APIResponseModel();
            try
            {
                var jewelryById = await _unitOfWork.JewelryRepository.GetByIdAsync(jewelryId);
                if (jewelryById != null)
                {
                    jewelryById.Valuation.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == status).Name;


                    AddHistoryValuation(jewelryById.Valuation.Id, jewelryById.Valuation.Status);
                    _unitOfWork.JewelryRepository.Update(jewelryById);
                    var notification = new Notification
                    {
                        Title = $"Final valuation Id:  {jewelryById.ValuationId} has been approved by manager",
                        Description = $"  định giá cuối cho valuation Id : {jewelryById.ValuationId} đã được manager approved",
                        Is_Read = false,
                        NotifiableId = jewelryById.ValuationId,  //jewelryId
                        AccountId = jewelryById.Valuation.Seller.AccountId,
                        CreationDate = DateTime.UtcNow
                    };

                    await _unitOfWork.NotificationRepository.AddAsync(notification);
                    await _unitOfWork.SaveChangeAsync();
                   

                    var jewelryDTO = _mapper.Map<JewelryDTO>(jewelryById);

                    response.Message = $"Update status Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = jewelryDTO;
                }
                else
                {
                    response.Message = $"Not found valuation";
                    response.Code = 404;
                    response.IsSuccess = true;
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> RequestOTPForAuthorizedBySellerAsync(int valuationId, int sellerId)
        {
            var response = new APIResponseModel();
            try
            {

                var seller = await _unitOfWork.CustomerRepository.GetByIdAsync(sellerId);

                if (seller == null)
                {
                    response.IsSuccess = false;
                    response.Message = "user not found";
                    response.Code = 404;
                }
                var otp = OtpService.GenerateOtpForAuthorized(seller.Account.ConfirmationToken, valuationId, sellerId);
                var emailSent = await SendEmail.SendEmailOTP(seller.Account.Email, otp);
                if (!emailSent)
                {
                    response.IsSuccess = false;
                    response.Message = "Error sending OTP email.";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Send OTP successfully, Please check email.";
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> VerifyOTPForAuthorizedBySellerAsync(int valuationId, int sellerId, string opt)
        {
            var response = new APIResponseModel();
            try
            {
                var seller = await _unitOfWork.CustomerRepository.GetByIdAsync(sellerId);
                var valuation = await _unitOfWork.ValuationRepository.GetByIdAsync(valuationId);
                var statusResult = OtpService.ValidateOtpForAuthorized(seller.Account.ConfirmationToken, valuationId, sellerId, opt);
                dynamic validationResult = statusResult;
                if (!validationResult.status)
                {
                    response.IsSuccess = false;
                    response.Message = validationResult.msg + validationResult.timeStepMatched;
                    return response;
                }
                else
                {
                    var status = EnumStatusValuation.Authorized.ToString();
                    valuation.Status = status;
                    
                    _unitOfWork.ValuationRepository.Update(valuation);

                    valuation.Jewelry.Status = EnumStatusJewelry.Authorized.ToString();
                    _unitOfWork.JewelryRepository.Update(valuation.Jewelry);
                    
                    
                    AddHistoryValuation(valuation.Id, status);
                    await _unitOfWork.SaveChangeAsync();

                    byte[] pdfBytes = _generatePDFService.CreateAuthorizedPDF(valuation);

                    string filePath = $"GiayUyQuyen_{valuation.Id}.pdf";

                    await File.WriteAllBytesAsync(filePath, pdfBytes);

                    var uploadFile = await _cloudinary.UploadAsync(new RawUploadParams
                    {
                        File = new FileDescription(filePath),
                        Tags = TagsAuthorized,
                        Type = "upload"

                    }).ConfigureAwait(false);
                    if (uploadFile == null || uploadFile.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        response.Message = $"File upload failed." + uploadFile.Error.Message + "";
                        response.Code = (int)uploadFile.StatusCode;
                        response.IsSuccess = false;
                    }
                    else
                    {
                        var valuationDoc = new ValuationDocumentDTO
                        {
                            ValuationId = valuation.Id,
                            ValuationDocumentType = "Authorized",
                            DocumentLink = uploadFile.SecureUrl.AbsoluteUri,
                            CreationDate = DateTime.Now,
                            CreatedBy = valuation.StaffId
                        };
                        var entity = _mapper.Map<ValuationDocument>(valuationDoc);
                        await _unitOfWork.ValuationDocumentRepository.AddAsync(entity);

                        var notification = new Notification
                        {
                            Title = $"Final valuation Id:  {valuation.Id} has been authorized by seller ",
                            Description = $"  định giá cuối cho valuation Id : {valuation.Id} has been authorized by seller",
                            Is_Read = false,
                            NotifiableId = valuation.Id,  //jewelryId
                            AccountId = valuation.Staff.AccountId,
                            CreationDate = DateTime.UtcNow
                        };

                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                        await _unitOfWork.SaveChangeAsync();
                    }
                    var valuationDTO = _mapper.Map<ValuationDTO>(valuation);
                    response.Message = "Authorized Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = valuationDTO;
                }               
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error";
            }
            return response;
        }

        public async Task<APIResponseModel> GetJewelryNoLotAsync(int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                

                var jewelrys = await _unitOfWork.JewelryRepository.GetAllJewelryNoLotAynsc(pageSize, pageIndex);
                List<JewelryListDTO> listjewelryDTO = new List<JewelryListDTO>();
                if (jewelrys.totalItem > 0)
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in jewelrys.data)
                    {
                        var jewelrysResponse = _mapper.Map<JewelryListDTO>(item);
                        listjewelryDTO.Add(jewelrysResponse);
                    };

                    var dataresponse = new
                    {
                        DataResponse = listjewelryDTO,
                        totalItemRepsone = jewelrys.totalItem
                    };

                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have valuations";
                    response.Code = 404;
                    response.IsSuccess = true;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> GetJewelryByCategoryAsync(int categoryId, int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                Expression<Func<Jewelry, bool>> filter;

                filter = x => x.CategoryId == categoryId;


                var jewelrys = await _unitOfWork.JewelryRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Category,ImageJewelries",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);


               
                List<JewelryListDTO> listjewelryDTO = new List<JewelryListDTO>();
                if (jewelrys.totalItems > 0)
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in jewelrys.data)
                    {
                        var jewelrysResponse = _mapper.Map<JewelryListDTO>(item);
                        listjewelryDTO.Add(jewelrysResponse);
                    };

                    var dataresponse = new
                    {
                        DataResponse = listjewelryDTO,
                        totalItemRepsone = jewelrys.totalItems
                    };

                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have valuations";
                    response.Code = 404;
                    response.IsSuccess = true;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> GetJewelryByArtistAsync(int artistId, int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                Expression<Func<Jewelry, bool>> filter;

                    filter = x => x.ArtistId == artistId;
                

                var jewelrys = await _unitOfWork.JewelryRepository.GetAllPaging(filter: filter,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Artist,ImageJewelries",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);


                List<JewelryListDTO> listjewelryDTO = new List<JewelryListDTO>();
                if (jewelrys.totalItems > 0)
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in jewelrys.data)
                    {
                        var jewelrysResponse = _mapper.Map<JewelryListDTO>(item);
                        listjewelryDTO.Add(jewelrysResponse);
                    };

                    var dataresponse = new
                    {
                        DataResponse = listjewelryDTO,
                        totalItemRepsone = jewelrys.totalItems
                    };

                    response.Data = dataresponse;
                }
                else
                {
                    response.Message = $"Don't have jewelry";
                    response.Code = 404;
                    response.IsSuccess = true;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }
    }
}
