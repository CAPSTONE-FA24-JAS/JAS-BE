using Application.ServiceReponse;
using Application.ViewModels.CustomerLotDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBidPriceService
    {
        Task<APIResponseModel> JoinBid(JoinLotRequestDTO request);

        Task<APIResponseModel> PlaceBiding(BiddingInputDTO request);

        Task<APIResponseModel> PlaceBidForReducedBidding(BiddingInputDTO request);

        Task<APIResponseModel> UpdateStatusBid(int lotId, int? status);


    }
}
