using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.KeyCharacteristicDTOs;
using AutoMapper;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ArtistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<APIResponseModel> CreateArtistAsync(ArtistDTO artistDTO)
        {
            var response = new APIResponseModel();
            try
            {

                var artist = _mapper.Map<Artist>(artistDTO);
                if (artist == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    await _unitOfWork.ArtistRepository.AddAsync(artist);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Create artist Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = artist;
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

        public async Task<APIResponseModel> getArtistAsync()
        {
            var response = new APIResponseModel();
            try
            {
                var artists = await _unitOfWork.ArtistRepository.GetAllPaging(filter: null,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate)
                                                                             );
                List<ArtistDTO> listArtistDTO = new List<ArtistDTO>();
                if (artists.totalItems > 0)
                {
                    response.Message = $"List artist Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in artists.data)
                    {
                        var artistsResponse = _mapper.Map<ArtistDTO>(item);
                        listArtistDTO.Add(artistsResponse);
                    };
                    response.Data = listArtistDTO;
                }
                else
                {
                    response.Message = $"Don't have artist";
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
