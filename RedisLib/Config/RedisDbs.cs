using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace RedisLib.Config
{
    public static class RedisDbs
    {
        private static readonly object _locker = new object();
        private static ConnectionMultiplexer _manager;
        private static ConnectionMultiplexer Manager
        {
            get
            {
                if (_manager == null)
                {
                    InnitManager();
                }
                return _manager;
            }
        }

        /// <summary>
        /// 1.redis只能本地访问
        /// 2.端口默认为1112
        /// 3.密码为20191112
        /// </summary>
        private static void InnitManager()
        {
            lock (_locker)
            {
                var options = ConfigurationOptions.Parse("127.0.0.1:1112");
                options.Password = "20191112";
                _manager = ConnectionMultiplexer.Connect(options);
            }           
        }


        private static IDatabase GetDb(RedisType redisType)
        {
            var intType = (int) redisType;
            return Manager.GetDatabase(intType);
        }


        /// <summary>
        /// 公共commonDb
        /// </summary>
        /// <returns></returns>
        public static IDatabase CommonDb()
        {
            return GetDb(RedisType.Common);
        }

        /// <summary>
        /// 基础框架人员部门Db
        /// </summary>
        /// <returns></returns>
        public static IDatabase AuditFrameDb()
        {
            return GetDb(RedisType.AuditFrame);
        }


        /// <summary>
        /// 车险系统独立Db
        /// </summary>
        /// <returns></returns>
        public static IDatabase InsuranceDb()
        {
            return GetDb(RedisType.Insurance);
        }
    }
}
