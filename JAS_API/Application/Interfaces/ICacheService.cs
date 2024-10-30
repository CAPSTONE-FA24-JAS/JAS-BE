﻿using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICacheService
    {
        T GetData<T>(string key);

        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

        object RemoveData(string key);

        bool SetSortedSetData<T>(string key, T value, float? score);

        //luu lot và sap xep theo time
        bool SetSortedSetDataForTime<T>(string key, T value, DateTime endTime);

        //get theo time theo thu tu giam dan
        List<T> GetSortedSetDataFilter<T>(string key, Func<T, bool> filter = null);

        List<T> GetSortedSetDataForTime<T>(string key, Func<T, bool> filter = null);

        void SetEndTime(int lotId, DateTime endTime);

        DateTime GetEndTime(int lotId);

        void SetLotInfoByAuction(Lot lot);
       
        Lot GetLotByIdInAuction(int lotId);

        void UpdateLotEndTimeInAuction(int lotId, DateTime newEndTime);

        List<Lot> GetHashLots(Func<Lot, bool> filter);
        void UpdateLotStatus(int lotId, string status);

        void UpdateMultipleLotsStatus(List<Lot> lotIds, string status);

        DateTime? GetMaxEndTimeByAuctionId(int? auctionId);

        void UpdateLotActualEndTime(int lotId, DateTime newEndTime);

        void UpdateLotCurrentPriceForReduceBidding(int lotId, float? currentPrice);


    }
}
