using Application.ServiceReponse;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.KeyCharacteristicDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IArtistService
    {
        public Task<APIResponseModel> CreateArtistAsync(ArtistDTO artistDTO);

        public Task<APIResponseModel> getArtistAsync();
    }
}
