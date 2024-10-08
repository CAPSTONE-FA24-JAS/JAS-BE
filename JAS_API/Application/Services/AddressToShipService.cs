using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.AddressToShipDTO;
using AutoMapper;
using Domain.Entity;
using Google.Apis.Util;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Services
{
    public class AddressToShipService : IAddressToShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressToShipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> CreateAddressToShip(CreateAddressToShipDTO createDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var entity = _mapper.Map<AddressToShip>(createDTO);
                await _unitOfWork.AddressToShipRepository.AddAsync(entity);
                if(await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.IsSuccess = true;
                    reponse.Message = "Create New Address Successfull";
                    reponse.Code = 200;
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Create New Address Faild";
                    reponse.Code = 400;
                }

            }catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> DeleteAddressToShip(int Id)
        {
            var reponse = new APIResponseModel();
            try
            {
                var entity = await _unitOfWork.AddressToShipRepository.GetByIdAsync(Id);
                if(entity != null)
                {
                     _unitOfWork.AddressToShipRepository.SoftRemove(entity);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "SoftRemove New Address Successfull";
                        reponse.Code = 200;
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "SoftRemove New Address Fail, When Saving";
                        reponse.Code = 400;
                    }
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "Not Found Address By Id";
                    reponse.Code = 400;
                }


            }
            catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return reponse;
        }

        public async Task<APIResponseModel> ViewListAddressToShip()
        {
            var reponse = new APIResponseModel();
            var DTOs = new List<ViewAddressToShipDTO>();
            try
            {
                var listAddressToShip = await _unitOfWork.AddressToShipRepository.GetAllAsync(includes: x => x.Ward);
                if (listAddressToShip != null)
                {
                    foreach(var a in listAddressToShip)
                    {
                        var mapper = _mapper.Map<ViewAddressToShipDTO>(a);
                        if (a.Ward != null)
                        {
                            mapper.WardName = a.Ward.Name;
                            if (a.Ward.District != null)
                            {
                                mapper.DistrictName = a.Ward.District.Name;
                                if (a.Ward.District.Province != null)
                                {
                                    mapper.ProvinceName = a.Ward.District.Province.Name;
                                }
                            }
                        }
                        DTOs.Add(mapper);
                    }
                    if(DTOs.Count > 0)
                    {
                        reponse.IsSuccess = true;
                        reponse.Message = "Received List Address Successfull";
                        reponse.Code = 200;
                        reponse.Data = DTOs;
                    }
                    else
                    {
                        reponse.IsSuccess = false;
                        reponse.Message = "List is empty";
                        reponse.Code = 400;
                        reponse.Data = DTOs;

                    }
                    
                }
                else
                {
                    reponse.IsSuccess = false;
                    reponse.Message = "List is null.";
                    reponse.Code = 400;
                }

            }
            catch (Exception ex)
            {
                reponse.IsSuccess = false;
                reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return reponse;
        }
    }
}
