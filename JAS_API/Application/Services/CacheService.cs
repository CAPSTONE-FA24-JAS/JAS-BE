
using Application.Interfaces;
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
    }
}
