using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedisLib.Model.Frame;
using StackExchange.Redis;

namespace RedisLib.Config
{
    public static class RedisHelper
    {
        #region Key
        /// <summary>
        /// key是否存在
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ExistsKey(IDatabase Db,string key)
        {
            return Db.KeyExists(key);
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool DeleteKey(IDatabase Db, string key)
        {
            return Db.KeyDelete(key,CommandFlags.DemandMaster);
        }

        /// <summary>
        /// 删除keys
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static long DeleteKeys(IDatabase Db, List<string> keys)
        {
            var redisKeys = keys.Select(s => (RedisKey) s).ToArray();
            return Db.KeyDelete(redisKeys);
        }

        #endregion


        #region String

        /// <summary>
        /// 设置一个可以过期的string
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes">过期时间，分钟</param>
        /// <returns></returns>
        public static bool SetString(IDatabase Db,string key,string value,int expireMinutes)
        {
            return Db.StringSet(key, value, TimeSpan.FromMinutes(expireMinutes));
        }

        /// <summary>
        /// 设置一个String，默认凌晨过期
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetString(IDatabase Db, string key, string value)
        {
            var now = DateTime.Now;
            var endTime = now.Date.AddDays(1);
            var timeSpan = (endTime - now);
            return Db.StringSet(key, value, timeSpan);
        }

        public static string GetString(IDatabase Db, string key)
        {
            return Db.StringGet(key);
        }


        #endregion


        #region List

        /// <summary>
        /// list后面追加
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="table"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long PushList(IDatabase Db, Enum table,object value)
        {          
            return Db.ListRightPush(table.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 设置全部list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Db"></param>
        /// <param name="table"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static long SetList<T>(IDatabase Db, Enum table, List<T> values)
        {
            var redisValues = values.Select(s => (RedisValue) Newtonsoft.Json.JsonConvert.SerializeObject(s)).ToArray();
            return Db.ListRightPush(table.ToString(), redisValues);
        }


        /// <summary>
        /// 获取全部list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Db"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(IDatabase Db, Enum table)
        {
            var value = Db.ListRange(table.ToString());
            return value.Select(s => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s)).ToList();
        }

        #endregion


        #region Hash

        /// <summary>
        /// 逐个添加hash
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool SetHash(IDatabase Db, Enum table, string key, object item)
        {            
            return Db.HashSet(table.ToString(), key, Newtonsoft.Json.JsonConvert.SerializeObject(item));
        }

        /// <summary>
        /// 获取特定
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Db"></param>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetHashItem<T>(IDatabase Db, Enum table, string key)
        {
            var ret = Db.HashGet(table.ToString(), key);
            if (!ret.HasValue) return default(T);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(ret);
        }

        /// <summary>
        /// 获取特定list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Db"></param>
        /// <param name="table"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static List<T> GetHashItems<T>(IDatabase Db, Enum table, List<string> keys)
        {
            var redistKeys = keys.Select(s => (RedisValue) s).ToArray();
            var rets = Db.HashGet(table.ToString(), redistKeys);
            return rets.Where(s => s.HasValue).Select(s => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s))
                .ToList();
        }

        public static List<T> GetHashAll<T>(IDatabase Db, Enum table)
        {
            return Db.HashGetAll(table.ToString()).Select(s => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(s.Value))
                .ToList();
        }


        #endregion


        /// <summary>
        /// 整体添加UserLogin
        /// </summary>
        /// <param name="Db"></param>
        /// <param name="items"></param>
        public static void SpecificSetUserLogin(List<RUserLogin> items)
        {
            var db = RedisDbs.AuditFrameDb();
            var hashItems = items.Select(s => new HashEntry(s.Name, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                .ToArray();
            db.HashSet(Tables.UserLogin.ToString(), hashItems);
        }

        public static void SpecificSetUserInfo(List<RUserInfo> items)
        {
            var db = RedisDbs.AuditFrameDb();
            var hashItems = items.Select(s =>
                new HashEntry(s.Id.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(s))).ToArray();
            db.HashSet(Tables.UserInfo.ToString(), hashItems);
        }

    }
}
