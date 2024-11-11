using Application.Interfaces;
using Application.ServiceReponse;
using Application.Utils;
using Application.ViewModels.KeyCharacteristicDTOs;
using Application.ViewModels.ValuationDTOs;
using AutoMapper;
using Domain.Entity;
using Domain.Enums;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class KeyCharacteristicService : IKeyCharacteristicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public KeyCharacteristicService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<APIResponseModel> CreateKeyCharacteristicAsync(KeyCharacteristicDTO keyCharacteristicDTO)
        {
            var response = new APIResponseModel();
            try
            {

                var keyCharacteristic = _mapper.Map<KeyCharacteristic>(keyCharacteristicDTO);
                if (keyCharacteristic == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    await _unitOfWork.KeyCharacteristicRepository.AddAsync(keyCharacteristic);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Create key Characteristic Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = keyCharacteristic;
                    }
                    else
                    {
                        response.Message = $"Save DB failed!";
                        response.Code = 500;
                        response.IsSuccess = false;
                    }
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

        public async Task<APIResponseModel> getKeyCharacteristicesAsync()
        {
            var response = new APIResponseModel();
            try
            {
                var keycharacteristices = await _unitOfWork.KeyCharacteristicRepository.GetAllPaging(filter: null,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate)
                                                                             );
                List<KeyCharacteristicDTO> listkeyDTO = new List<KeyCharacteristicDTO>();
                if (keycharacteristices.totalItems > 0)
                {
                    response.Message = $"List key characteristices Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in keycharacteristices.data)
                    {
                        var keysResponse = _mapper.Map<KeyCharacteristicDTO>(item);
                        listkeyDTO.Add(keysResponse);
                    };
                    response.Data = listkeyDTO;
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
    }
}
