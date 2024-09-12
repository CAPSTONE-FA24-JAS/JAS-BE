using Application.Interfaces;
using Application.Repositories;
using Application.ServiceReponse;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using Azure;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Core;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ValuationService : IValuationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_ImageValuation";
        public ValuationService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }
        public async Task<APIResponseModel> ConsignAnItem(ConsignAnItemDTO consignAnItem)
        {
            var response = new APIResponseModel();
            List<String> imagesValuation = new List<string>();
            try
            {
                var newvaluation = _mapper.Map<Valuation>(consignAnItem);
                if (newvaluation == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    newvaluation.Status = "Requested";
                    await _unitOfWork.ValuationRepository.AddAsync(newvaluation);
                    await _unitOfWork.SaveChangeAsync();

                    foreach (var image in consignAnItem.ImageValuation)
                    {
                        var uploadImage = await _cloudinary.UploadAsync(new CloudinaryDotNet.Actions.ImageUploadParams
                        {
                            File = new FileDescription(image.FileName,
                                                   image.OpenReadStream()),
                            Tags = Tags
                        }).ConfigureAwait(false);



                        if (uploadImage == null || uploadImage.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            response.Message = $"Image upload failed." + uploadImage.Error.Message + "";
                            response.Code = (int)uploadImage.StatusCode;
                            response.IsSuccess = false;
                        }
                        else
                        {
                            var imageValuationinput = new ImageValuationDTO
                            {
                                ValuationId = newvaluation.Id,
                                ImageLink = uploadImage.SecureUrl.AbsoluteUri
                            };
                            imagesValuation.Add(imageValuationinput.ImageLink);
                            var imageValuation = _mapper.Map<ImageValuation>(imageValuationinput);
                            await _unitOfWork.ImageValuationRepository.AddAsync(imageValuation);
                            if (await _unitOfWork.SaveChangeAsync() > 0)
                            {
                                response.Message = $"Image upload Successfull";
                                response.Code = 200;
                                response.IsSuccess = true;
                            }

                        }
                    }

                    response.Message = $"Consign an item Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    response.Data = imagesValuation;

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
