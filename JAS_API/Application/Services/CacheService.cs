
using Application.Interfaces;
using Domain.Entity;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;

        public CacheService()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect("localhost:6379");
                _cacheDb = redis.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kết nối Redis: {ex.Message}");
            }
        }


        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);

            return default;

        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
                return _cacheDb.KeyDelete(key);
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expriryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expriryTime);


        }

        public bool SetSortedSetData<T>(string key, T value, int score)
        {

            //   var expriryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var data = _cacheDb.SortedSetAdd(key, JsonSerializer.Serialize(value), score);
            if (data)
                return true;
            return false;

        }

        public bool SetSortedSetDataForTime<T>(string key, T value, DateTime endTime)
        {
            var timestamp = new DateTimeOffset(endTime).ToUnixTimeSeconds();
            var data = _cacheDb.SortedSetAdd(key, JsonSerializer.Serialize(value), timestamp);
            if (data)
                return true;
            return false;
        }


        //get sortedset k filter
        public List<T> GetSortedSetData<T>(string key)
        {
            var value = _cacheDb.SortedSetRangeByRank(key, 0, -1, Order.Descending);

            var result = new List<T>();
            foreach (var item in value)
            {
                if (item.HasValue)
                {
                    var deserializedItem = JsonSerializer.Deserialize<T>(item);
                    result.Add(deserializedItem);
                }
            }

            return result;

        }

        //get sortedSet filter
        public List<T> GetSortedSetDataForTime<T>(string key, Func<T, bool> filter = null)
        {
            var value = _cacheDb.SortedSetRangeByRank(key, 0, -1, Order.Descending);

            var result = new List<T>();
            foreach (var item in value)
            {
                if (item.HasValue)
                {
                    var deserializedItem = JsonSerializer.Deserialize<T>(item);
                    if(filter == null || filter(deserializedItem))
                    {
                        result.Add(deserializedItem);
                    }                   
                }
            }

            return result;

        }

        public void SetEndTime(int lotId, DateTime endTime)
        {
            var key = $"lot:{lotId}:endTime";
            var endTimeString = endTime.ToString("o");
            
            _cacheDb.StringSet(key, endTimeString);
        }

        public DateTime GetEndTime(int lotId)
        {
            var key = $"lot:{lotId}:endTime";
            var endTimeString = _cacheDb.StringGet(key);

            if (!string.IsNullOrEmpty(endTimeString))
            {
                return DateTime.Parse(endTimeString);
            }
            return DateTime.Parse(endTimeString);
        }


        //lưu thong tin lot vao redis bang hash, còn endTime thi dung sortedSet
        public void SetLotInfo(Lot lot)
        {
            var lotHashKey = $"lot-{lot.Id}";
            var lotData = JsonSerializer.Serialize(lot);
            _cacheDb.HashSet(lotHashKey, "lot", lotData);
            var timestamp = new DateTimeOffset(lot.EndTime).ToUnixTimeSeconds();
            _cacheDb.SortedSetAdd("LotEndTime", lot.Id.ToString(), timestamp);
        }

        //get lot theo lot id
        public Lot GetLotById(int lotId)
        {
            var lotHashKey = $"lot-{lotId}"; // Tạo khóa cho Lot trong Redis
            var lotData = _cacheDb.HashGet(lotHashKey, "lot"); // Lấy dữ liệu Lot từ Redis Hash

            // Kiểm tra xem dữ liệu có tồn tại không
            if (lotData.HasValue)
            {
                return JsonSerializer.Deserialize<Lot>(lotData); // Chuyển dữ liệu JSON thành đối tượng Lot
            }
            return null; // Trả về null nếu không tìm thấy Lot
        }

        public void UpdateLotEndTime(int lotId, DateTime newEndTime)
        {
            var lotHashKey = $"lot-{lotId}"; // Tạo khóa cho Lot trong Redis
            var lotData = _cacheDb.HashGet(lotHashKey, "lot");

            if (lotData.HasValue)
            {
                var lot = JsonSerializer.Deserialize<Lot>(lotData);
                lot.EndTime = newEndTime;

                var updateLot = JsonSerializer.Serialize(lot);

                _cacheDb.HashSet(lotHashKey, "lot", updateLot);
            }
        }

        public void UpdateLotStatus(int lotId, string status)
        {
            var lotHashKey = $"lot-{lotId}"; // Tạo khóa cho Lot trong Redis
            var lotData = _cacheDb.HashGet(lotHashKey, "lot");

            if (lotData.HasValue)
            {
                var lot = JsonSerializer.Deserialize<Lot>(lotData);
                lot.Status = status;

                var updateLot = JsonSerializer.Serialize(lot);

                _cacheDb.HashSet(lotHashKey, "lot", updateLot);
            }
        }


        //get all lot loc theo filter, kieu hash
        public List<Lot> GetHashLots(Func<Lot, bool> filter)
        {
            var lots = new List<Lot>();

            // Giả sử các Lot được lưu với khóa dạng "lot-{lotId}"
            var server = _cacheDb.Multiplexer.GetServer(_cacheDb.Multiplexer.GetEndPoints().First());

            // Lấy tất cả các khóa có dạng "lot-*"
            var lotKeys = server.Keys(pattern: "lot-*");

            foreach (var lotKey in lotKeys)
            {
                // Lấy dữ liệu của mỗi Lot từ Redis
                var lotData = _cacheDb.HashGet(lotKey, "data");

                if (lotData.HasValue)
                {
                    // Chuyển chuỗi JSON thành đối tượng Lot
                    var lot = JsonSerializer.Deserialize<Lot>(lotData);
                    lots.Add(lot);
                }
            }

            // Lọc danh sách lot theo filter
            return lots.Where(filter).ToList();
        }





    }
}
