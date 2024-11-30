
using Application.Interfaces;
using Application.ViewModels.BidPriceDTOs;
using Application.ViewModels.CustomerLotDTOs;
using CloudinaryDotNet;
using Domain.Entity;
using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly ISubscriber _subscriber;

        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            try
            {
                
                _cacheDb = connectionMultiplexer.GetDatabase();
                _subscriber = connectionMultiplexer.GetSubscriber();
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

        //        public bool PlaceBidWithLuaScript(int lotId, BiddingInputDTO request, int customerId)
        //        {
        //            string script = @"
        //    local key = KEYS[1]
        //    local newPrice = tonumber(ARGV[1])
        //    local newTime = ARGV[2]
        //    local bidData = ARGV[3]

        //    -- Lấy giá cao nhất hiện tại
        //    local highestBid = redis.call('ZRANGE', key, -1, -1, 'WITHSCORES')
        //    if #highestBid > 0 then
        //        local highestBidData = cjson.decode(highestBid[1])
        //        local highestBidPrice = tonumber(highestBid[2])
        //        local highestBidTime = highestBidData.BidTime

        //        -- Kiểm tra điều kiện giá mới phải lớn hơn giá cao nhất
        //        if newPrice > highestBidPrice then
        //            -- Chấp nhận giá mới và thêm vào SortedSet
        //            redis.call('ZADD', key, 'GT', newPrice, bidData)
        //            return 1
        //        elseif newPrice == highestBidPrice and newTime > highestBidTime then
        //            -- Không chấp nhận giá nếu giá mới bằng và thời gian sau hơn
        //            return 0
        //        elseif newPrice < highestBidPrice and newTime > highestBidTime then
        //            -- Không chấp nhận giá nếu giá mới nho và thời gian sau hơn
        //            return 0
        //        else
        //            return 0
        //        end
        //    else
        //        -- Nếu chưa có giá nào, chấp nhận giá đầu tiên
        //        redis.call('ZADD', key, newPrice, bidData)
        //        return 1
        //    end
        //";

        //            // Chuỗi JSON của BidPrice
        //            var bidData = JsonSerializer.Serialize(new BidPrice
        //            {
        //                CurrentPrice = request.CurrentPrice,
        //                BidTime = request.BidTime,
        //                CustomerId = customerId,
        //                LotId = lotId
        //            });
        //            var timestamp = new DateTimeOffset(request.BidTime).ToUnixTimeSeconds();
        //            // Key Redis dựa trên LotId
        //            string redisKey = $"BidPrice:{lotId}";

        //            // Thực hiện Lua script trong Redis
        //            var result = (int)_cacheDb.ScriptEvaluate(script, new RedisKey[] { redisKey },
        //                new RedisValue[] { request.CurrentPrice, timestamp, bidData });

        //            return result == 1;
        //        }

        public (bool result, BidPrice? bidPrice, float? highestBid) PlaceBidWithLuaScript(int lotId)
        {
            string script = @"
local stream_key = KEYS[1]
local sorted_set_key = KEYS[2]
local last_processed_id_key = KEYS[3]

-- Lấy giá đấu đầu tiên từ Stream
local entries = redis.call('XREAD', 'COUNT', 1, 'STREAMS', stream_key, '0')
if not entries or #entries == 0 then
    return nil -- Không có giá đấu nào trong Stream
else
 local entry_id = entries[1][2][1][1]
 local bid_data = entries[1][2][1][2]

local bid_data = cjson.decode(bid_data[2])  -- Decode the JSON data
local newPrice = tonumber(bid_data.CurrentPrice)  -- Extract the new price
local newTime = bid_data.BidTime

-- Lấy giá đấu cao nhất hiện tại từ Sorted Set
local highestBid = redis.call('ZRANGE', sorted_set_key, -1, -1, 'WITHSCORES')
local highestBidPrice = 0
local highestBidTime = 0

if #highestBid > 0 then
    highestBidPrice = tonumber(highestBid[2])
    highestBidTime = cjson.decode(highestBid[1]).BidTime
    -- Kiểm tra điều kiện giá mới
    if newPrice > highestBidPrice then
        bid_data.Status = ""Success""
        redis.call('ZADD', sorted_set_key, newPrice, cjson.encode(bid_data))
        redis.call('XDEL', stream_key, entry_id) -- Xóa khỏi Stream nếu đạt điều kiện    
    elseif newPrice == highestBidPrice then
        bid_data.Status = ""Failed""
        redis.call('ZADD', sorted_set_key, newPrice, cjson.encode(bid_data))
        redis.call('XDEL', stream_key, entry_id)
    elseif newPrice < highestBidPrice then
        bid_data.Status = ""Failed""
        redis.call('ZADD', sorted_set_key, newPrice, cjson.encode(bid_data))
        redis.call('XDEL', stream_key, entry_id)
    end
else
    -- Chấp nhận giá đầu tiên nếu chưa có giá nào
    bid_data.Status = ""Success""
    redis.call('ZADD', sorted_set_key, newPrice, cjson.encode(bid_data))
    redis.call('XDEL', stream_key, entry_id) -- Xóa khỏi Stream   
end


highestBid = redis.call('ZRANGE', sorted_set_key, -1, -1, 'WITHSCORES')
if #highestBid > 0 then
    return {highestBid[2], cjson.encode(bid_data)}
else
    return nil
end
end
"
            ;

           

            // Key Redis dựa trên LotId
            string streamKey = $"BidStream:{lotId}";
            string sortedSetKey = $"BidPrice:{lotId}";
            var result = _cacheDb.ScriptEvaluate(script, new RedisKey[] { streamKey, sortedSetKey });

            if (result.Type == ResultType.MultiBulk)
            {
                try
                {
                    var resultArray = (RedisResult[])result;

                    // Kiểm tra có đủ 2 phần tử trong kết quả trả về
                    if (resultArray.Length == 2)
                    {
                        var highestBid = (float?)Convert.ToDouble(resultArray[0]);  // Giá đấu cao nhất
                        var bidDataJson = (string)resultArray[1]; // Chuỗi JSON của bid_data

                        if (!string.IsNullOrEmpty(bidDataJson))
                        {
                            // Giải mã bid_data thành đối tượng BidPriceDTO
                            BidPrice bidPrice = JsonSerializer.Deserialize<BidPrice>(bidDataJson);
                            return (true, bidPrice, highestBid); // Trả về kết quả
                        }
                    }
                    else
                    {
                        return (false, null, null); // Nếu không có kết quả hợp lệ
                    }
                }
                catch(JsonException ex)
                {
                    Console.WriteLine($"Error deserializing resultJson: {ex.Message}");
                }
                // RedisResult sẽ là một mảng, nhưng nếu chỉ có một phần tử thì kết quả chỉ có một phần tử
                
            }
            else if (result.Type == ResultType.BulkString)
            {
                // Trường hợp result là một chuỗi JSON đơn
                var resultJson = result.ToString();

                if (!string.IsNullOrEmpty(resultJson) && resultJson != "{}")
                {
                    // Deserialize JSON thành đối tượng
                    var resultObj = JsonSerializer.Deserialize<BidResult[]>(resultJson);

                    if (resultObj != null)
                    {
                        var bidPrice = resultObj[0].BidPrice;
                        var highestBid = resultObj[1].HighestBid;
                        return (true, bidPrice, highestBid); // Trả về giá đấu và giá đấu cao nhất
                    }
                }
            }

            // Trường hợp không hợp lệ
            return (false, null, null);


        }


        public class BidResult
        {           
            public float? HighestBid { get; set; }
            public BidPrice? BidPrice { get; set; }
        }
        public BidPrice AddToStream(int lotId, BiddingInputDTO request, int customerId)
        {
            var streamKey = $"BidStream:{lotId}";
            var pubsubChannel = $"channel-{lotId}";
            var timestamp = new DateTimeOffset(request.BidTime).ToUnixTimeSeconds();
            var bidPrice = new BidPrice
            {
                CurrentPrice = request.CurrentPrice,
                BidTime = DateTime.UtcNow,
                Status = "Processing",
                CustomerId = customerId,
                LotId = lotId
            };
            var serializedBidPrice = JsonSerializer.Serialize(bidPrice);
            var entryId = _cacheDb.StreamAddAsync(streamKey, new NameValueEntry[]
            {
             new NameValueEntry("BidPrice", serializedBidPrice)  // Store the serialized BidPrice object
            });

            // Gửi tín hiệu Pub/Sub để thông báo
            var subscriber = _subscriber;
            subscriber.PublishAsync(pubsubChannel, "Newbid");
            return bidPrice;
        }

        //dki de nhan thong bao qua redis pub/sub
        public async Task SubscribeToChannelAsync(string channel, Action<string> messageHandler)
        {
            var subscriber = _subscriber;
            await subscriber.SubscribeAsync(channel, (channel, message) =>
            {
                messageHandler(message);
            });
        }

        public void UpdateLotRound(int lotId, int round)
        {
            var lotHashKey = $"lot-{lotId}"; // Tạo khóa cho Lot trong Redis
            var lotData = _cacheDb.HashGet(lotHashKey, "lot");

            if (lotData.HasValue)
            {
                var lot = JsonSerializer.Deserialize<Lot>(lotData);
                lot.Round = round;

                var updateLot = JsonSerializer.Serialize(lot);

                _cacheDb.HashSet(lotHashKey, "lot", updateLot);
            }
        }


    }
}
