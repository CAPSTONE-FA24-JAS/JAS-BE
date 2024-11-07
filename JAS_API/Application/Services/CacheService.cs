
using Application.Interfaces;
using Application.ViewModels.CustomerLotDTOs;
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

        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            try
            {
                
                _cacheDb = connectionMultiplexer.GetDatabase();
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

        public bool SetSortedSetData<T>(string key, T value, float? score)
        {

            //   var expriryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var data = _cacheDb.SortedSetAdd(key, JsonSerializer.Serialize(value), (double)score);
            if (data)
                return true;
            return false;

        }

        public bool SetSortedSetDataForBidPrice<T>(int lotId, T value, float? score) where T : BidPrice
        {
            // Chuỗi hóa đối tượng `BidPrice` để lưu trữ
            var bidData = JsonSerializer.Serialize(value);

            // Tạo key Redis dựa trên LotId
            string redisKey = $"BidPrice:{lotId}";

            // Lấy giá đấu cao nhất hiện tại từ Redis
            var highestBidData = _cacheDb.SortedSetRangeByScore(redisKey, order: Order.Descending, take: 1).FirstOrDefault();

            if (highestBidData.HasValue)
            {
                // Giải tuần tự giá đấu cao nhất thành đối tượng `BidPrice`
                var highestBid = JsonSerializer.Deserialize<BidPrice>(highestBidData.ToString());

                // Kiểm tra điều kiện: giá đấu mới phải cao hơn giá cao nhất hiện tại
                if (score <= highestBid.CurrentPrice)
                {
                    // Giá đấu không hợp lệ nếu giá không cao hơn giá hiện tại
                    return false;
                }
            }

            // Thêm giá đấu vào SortedSet nếu thỏa mãn điều kiện
            var result = _cacheDb.SortedSetAdd(redisKey, bidData, (double)score);
            return result;
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
        public List<T> GetSortedSetDataFilter<T>(string key, Func<T, bool>? filter = null)
        {
            var value = _cacheDb.SortedSetRangeByRank(key, 0, -1, Order.Descending);

            var result = new List<T>();
            foreach (var item in value)
            {
                if (item.HasValue)
                {
                    var deserializedItem = JsonSerializer.Deserialize<T>(item);
                    if (filter == null || filter(deserializedItem))
                    {
                        result.Add(deserializedItem);
                    }
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
            if (lot.EndTime.HasValue)
            {
                var timestamp = new DateTimeOffset(lot.EndTime.Value).ToUnixTimeSeconds();
                _cacheDb.SortedSetAdd("LotEndTime", lot.Id.ToString(), timestamp);
            }
           
            
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

        public void UpdateLotActualEndTime(int lotId, DateTime newEndTime)
        {
            var lotHashKey = $"lot-{lotId}"; // Tạo khóa cho Lot trong Redis
            var lotData = _cacheDb.HashGet(lotHashKey, "lot");

            if (lotData.HasValue)
            {
                var lot = JsonSerializer.Deserialize<Lot>(lotData);
                lot.ActualEndTime = newEndTime;

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

        public void UpdateLotCurrentPriceForReduceBidding(int lotId, float? currentPrice)
        {
            var lotHashKey = $"lot-{lotId}"; // Tạo khóa cho Lot trong Redis
            var lotData = _cacheDb.HashGet(lotHashKey, "lot");

            if (lotData.HasValue)
            {
                var lot = JsonSerializer.Deserialize<Lot>(lotData);
                lot.CurrentPrice = currentPrice;

                var updateLot = JsonSerializer.Serialize(lot);

                _cacheDb.HashSet(lotHashKey, "lot", updateLot);
            }
        }

        //update Status hàng loạt lên redis
        public void UpdateMultipleLotsStatus(List<Lot> lotIds, string status)
        {
            var batch = _cacheDb.CreateBatch();

            foreach (var lot in lotIds)
            {
                var lotHashKey = $"lot-{lot.Id}"; // Tạo khóa cho mỗi Lot trong Redis
                var lotData = _cacheDb.HashGet(lotHashKey, "lot");

                if (lotData.HasValue)
                {
                    var lotDes = JsonSerializer.Deserialize<Lot>(lotData);
                    lotDes.Status = status;

                    var updateLot = JsonSerializer.Serialize(lotDes);

                    // Sử dụng batch để cập nhật hàng loạt
                    batch.HashSetAsync(lotHashKey, "lot", updateLot);
                }
            }

            batch.Execute(); // Thực hiện tất cả các thao tác trong batch
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
                var lotData = _cacheDb.HashGet(lotKey, "lot");

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


        //Lưu vào Redis bằng Lua script với bidPrice

        public bool PlaceBidWithLuaScript(int lotId, BiddingInputDTO request, int customerId)
        {
            string script = @"
    local key = KEYS[1]
    local newPrice = tonumber(ARGV[1])
    local newTime = ARGV[2]
    local bidData = ARGV[3]
    
    -- Lấy giá cao nhất hiện tại
    local highestBid = redis.call('ZRANGE', key, -1, -1, 'WITHSCORES')
    if #highestBid > 0 then
        local highestBidData = cjson.decode(highestBid[1])
        local highestBidPrice = tonumber(highestBid[2])
        local highestBidTime = highestBidData.BidTime

        -- Kiểm tra điều kiện giá mới phải lớn hơn giá cao nhất
        if newPrice > highestBidPrice then
            -- Chấp nhận giá mới và thêm vào SortedSet
            redis.call('ZADD', key, newPrice, bidData)
            return 1
        elseif newPrice == highestBidPrice and newTime > highestBidTime then
            -- Không chấp nhận giá nếu giá mới bằng và thời gian sau hơn
            return 0
        elseif newPrice < highestBidPrice and newTime > highestBidTime then
            -- Không chấp nhận giá nếu giá mới nho và thời gian sau hơn
            return 0
        else
            return 0
        end
    else
        -- Nếu chưa có giá nào, chấp nhận giá đầu tiên
        redis.call('ZADD', key, newPrice, bidData)
        return 1
    end
";

            // Chuỗi JSON của BidPrice
            var bidData = JsonSerializer.Serialize(new BidPrice
            {
                CurrentPrice = request.CurrentPrice,
                BidTime = request.BidTime,
                CustomerId = customerId,
                LotId = lotId
            });

            // Key Redis dựa trên LotId
            string redisKey = $"BidPrice:{lotId}";

            // Thực hiện Lua script trong Redis
            var result = (int)_cacheDb.ScriptEvaluate(script, new RedisKey[] { redisKey },
                new RedisValue[] { request.CurrentPrice, request.BidTime.ToString("yyyy-MM-dd HH:mm:ss"), bidData });

            return result == 1;
        }

    }
}
