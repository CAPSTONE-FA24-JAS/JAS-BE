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

        public JewelryService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
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
                jewelry.CreationDate = DateTime.Now;
                jewelry.Valuation.Status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == 6).Name;

                AddHistoryValuation(jewelry.Valuation.Id, jewelry.Valuation.Status);

                await _unitOfWork.JewelryRepository.AddAsync(jewelry);
                await _unitOfWork.SaveChangeAsync();
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
                    foreach (var key in jewelryDTO.KeyCharacteristicDetails)
                    {

                        var keyDTO = _mapper.Map<KeyCharacteristicDetail>(key);
                        keyDTO.JewelryId = jewelry.Id;
                        await _unitOfWork.KeyCharacteristicsDetailRepository.AddAsync(keyDTO);
                        await _unitOfWork.SaveChangeAsync();
                    }
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
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageMianDiamond);

                    var imageMainDiamond = new ImageDiamondDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                        DiamondId = diamondId
                    };

                    var imageMainDiamondDTO = _mapper.Map<ImageMainDiamond>(imageMainDiamond);
                    await _unitOfWork.ImageMainDiamondRepository.AddAsync(imageMainDiamondDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
            }

            if (documents != null)
            {
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentMainDiamond);

                    var documentMainDiamond = new DocumentDiamondDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Diamond",
                        DiamondId = diamondId
                    };

                    var documentMainDiamondDTO = _mapper.Map<DocumentMainDiamond>(documentMainDiamond);
                    await _unitOfWork.DocumentMainDiamondRepository.AddAsync(documentMainDiamondDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
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
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageSecondDiamond);

                    var imageSecondDiamond = new ImageDiamondDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                        DiamondId = diamondId
                    };

                    var imageSecondDiamondDTO = _mapper.Map<ImageSecondaryDiamond>(imageSecondDiamond);
                    await _unitOfWork.ImageSecondDiamondRepository.AddAsync(imageSecondDiamondDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
            }

            if (documents != null)
            {
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentSecondDiamond);

                    var documentSecondDiamond = new DocumentDiamondDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Diamond",
                        DiamondId = diamondId
                    };

                    var documentSecondDiamondDTO = _mapper.Map<DocumentSecondaryDiamond>(documentSecondDiamond);
                    await _unitOfWork.DocumentSecondaryDiamondRepository.AddAsync(documentSecondDiamondDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
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
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageMainShaphie);

                    var imageMainShaphie = new ImageShaphieDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                        ShaphieId = shaphieId
                    };

                    var imageMainShaphieDTO = _mapper.Map<ImageMainShaphie>(imageMainShaphie);
                    await _unitOfWork.ImageMainShaphieRepository.AddAsync(imageMainShaphieDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
            }

            if (documents != null)
            {
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentMainShaphie);

                    var documentMainShaphie = new DocumentShaphieDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Shaphie",
                        ShaphieId = shaphieId
                    };

                    var documentMainShaphieDTO = _mapper.Map<DocumentMainShaphie>(documentMainShaphie);
                    await _unitOfWork.DocumentMainShaphieRepository.AddAsync(documentMainShaphieDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
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
                foreach (var image in images)
                {
                    var uploadImage = await uploadImageOnCloudary(image, TagsImageSecondShaphie);

                    var imageSecondShaphie = new ImageShaphieDTO
                    {
                        ImageLink = uploadImage.SecureUrl.AbsoluteUri,
                        ShaphieId = shaphieId
                    };

                    var imageSecondShaphieDTO = _mapper.Map<ImageSecondaryShaphie>(imageSecondShaphie);
                    await _unitOfWork.ImageSecondaryShaphieRepository.AddAsync(imageSecondShaphieDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
            }

            if (documents != null)
            {
                foreach (var document in documents)
                {
                    var uploadImage = await uploadImageOnCloudary(document, TagsDocumentSecondShaphie);

                    var documentSecondShaphie = new DocumentShaphieDTO
                    {
                        DocumentLink = uploadImage.SecureUrl.AbsoluteUri,
                        DocumentTitle = "Document of Shaphie",
                        ShaphieId = shaphieId
                    };

                    var documentSecondShaphieDTO = _mapper.Map<DocumentSecondaryShaphie>(documentSecondShaphie);
                    await _unitOfWork.DocumentSecondaryShaphieRepository.AddAsync(documentSecondShaphieDTO);
                    await _unitOfWork.SaveChangeAsync();
                }
            }


        }

        public async Task<APIResponseModel> GetJewelryAsync(int? pageSize, int? pageIndex)
        {
            var response = new APIResponseModel();
            try
            {
                var jewelrys = await _unitOfWork.JewelryRepository.GetAllPaging(filter: null,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate),
                                                                             includeProperties: "Artist,Category,ImageJewelries,KeyCharacteristicDetails,Lot,MainDiamonds,SecondaryDiamonds,MainShaphies,SecondaryShaphies,Valuation",
                                                                             pageIndex: pageIndex,
                                                                             pageSize: pageSize);
                List<JewelryDTO> listjewelryDTO = new List<JewelryDTO>();
                if (jewelrys.totalItems > 0)
                {
                    response.Message = $"List consign items Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in jewelrys.data)
                    {
                        var jewelrysResponse = _mapper.Map<JewelryDTO>(item);
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
                    valuationById.BidForm = requestDTO.BidForm;

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


                    AddHistoryValuation(jewelryId, jewelryById.Valuation.Status);
                    _unitOfWork.JewelryRepository.Update(jewelryById);
                    await _unitOfWork.SaveChangeAsync();

                    var valuationDTO = _mapper.Map<JewelryDTO>(jewelryById);

                    response.Message = $"Update status Successfully";
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

        public async Task<APIResponseModel> RequestOTPForAuthorizedBySellerAsync(int jewelryId, int sellerId)
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
                var otp = OtpService.GenerateOtpForAuthorized(seller.Account.ConfirmationToken, jewelryId, sellerId);
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

        public async Task<APIResponseModel> VerifyOTPForAuthorizedBySellerAsync(int jewelryId, int sellerId, string opt)
        {
            var response = new APIResponseModel();
            try
            {
                var seller = await _unitOfWork.CustomerRepository.GetByIdAsync(sellerId);
                var jewelry = await _unitOfWork.JewelryRepository.GetByIdAsync(jewelryId);
                var statusResult = OtpService.ValidateOtpForAuthorized(seller.Account.ConfirmationToken, jewelryId, sellerId, opt);
                dynamic validationResult = statusResult;
                if (!validationResult.status)
                {
                    response.IsSuccess = false;
                    response.Message = validationResult.msg + validationResult.timeStepMatched;
                    return response;
                }
                else
                {
                    var status = EnumHelper.GetEnums<EnumStatusValuation>().FirstOrDefault(x => x.Value == 8).Name;
                    jewelry.Valuation.Status = status;
                    var valuation = _mapper.Map<Valuation>(jewelry.Valuation);
                    _unitOfWork.ValuationRepository.Update(valuation);

                    AddHistoryValuation(valuation.Id, status);
                    await _unitOfWork.SaveChangeAsync();

                    byte[] pdfBytes = CreateAuthorizedPDFFile.CreateAuthorizedPDF(jewelry.Valuation);

                    string filePath = $"GiayUyQuyen_{jewelry.Valuation.Id}.pdf";

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
                            ValuationId = jewelry.Valuation.Id,
                            ValuationDocumentType = "Authorized",
                            DocumentLink = uploadFile.SecureUrl.AbsoluteUri,
                            CreationDate = DateTime.Now,
                            CreatedBy = jewelry.Valuation.StaffId
                        };
                        var entity = _mapper.Map<ValuationDocument>(valuationDoc);
                        await _unitOfWork.ValuationDocumentRepository.AddAsync(entity);
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
    }
}
