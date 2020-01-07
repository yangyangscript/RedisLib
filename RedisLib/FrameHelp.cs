using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedisLib.Model.Frame;
using StackExchange.Redis;

namespace RedisLib
{
    /// <summary>
    /// 框架常用方法
    /// </summary>
    public static class FrameHelp
    {

        /// <summary>
        /// 清空所有日常Key
        /// </summary>
        public static void DeleteDailyKeys()
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                RedisLib.Config.RedisHelper.DeleteKeys(db,
                    new List<string>()
                    {
                        RedisLib.Model.Frame.Tables.UserLogin.ToString(), RedisLib.Model.Frame.Tables.Dept.ToString()
                    });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// 部署所有可以登陆用户
        /// </summary>
        /// <param name="userLogins"></param>
        public static void InnitLoginUser(List<RUserLogin> userLogins)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                var hashItems = userLogins.Select(s => new HashEntry(s.Name, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                    .ToArray();
                db.HashSet(Tables.UserLogin.ToString(), hashItems);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// 跟新单个可以登陆用户
        /// </summary>
        /// <param name="userLogin"></param>
        public static void UpdateLoginUser(RUserLogin userLogin)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                RedisLib.Config.RedisHelper.SetHash(db, RedisLib.Model.Frame.Tables.UserLogin, userLogin.Name, userLogin);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 获取登陆用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static RUserLogin GetLoginUser(string loginName)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                return RedisLib.Config.RedisHelper.GetHashItem<RUserLogin>(db, Tables.UserLogin, loginName);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static List<RUserLogin> GetAllLoginUser()
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                return RedisLib.Config.RedisHelper.GetHashAll<RUserLogin>(db, Tables.UserLogin);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 部署所有用户
        /// </summary>
        /// <param name="userInfos"></param>
        public static void InnitUserInfos(List<RUserInfo> userInfos)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                var hashItems = userInfos.Select(s => new HashEntry(s.Id, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                    .ToArray();
                db.HashSet(Tables.UserInfo.ToString(), hashItems);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// 更新单个用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        public static void UpdateUserInfo(RUserInfo userInfo)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                RedisLib.Config.RedisHelper.SetHash(db, RedisLib.Model.Frame.Tables.UserInfo, userInfo.Id.ToString(), userInfo);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// 部署所有部门
        /// </summary>
        /// <param name="depts"></param>
        public static void InnitDepts(List<RDept> depts)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();               
                var hashItems = depts.Select(s => new HashEntry(s.Id, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                    .ToArray();
                db.HashSet(Tables.Dept.ToString(), hashItems);
            }
            catch (Exception e)
            {
                throw new  Exception(e.Message);
            }
        }

        /// <summary>
        /// 部门树递归方法
        /// </summary>
        /// <param name="dept"></param>
        /// <param name="items"></param>
        /// <param name="ret"></param>
        internal static void CompeleteDeptTree(RDept dept, List<RDept> items, List<RDept> ret)
        {
            ret.Add(dept);
            var sons =
                items.Where(s => s.TreePath.StartsWith(dept.TreePath) && s.TreeLevel == dept.TreeLevel + 1)
                    .OrderBy(s => s.SortIndex)
                    .ToList();
            if (!sons.Any()) return;
            foreach (var son in sons)
            {
                CompeleteDeptTree(son, items, ret);
            }
        }

        /// <summary>
        /// 获取部门所有下级Ids
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        internal static List<int> CacheDeptChildren(int deptId)
        {
            var allDepts = GetDepts();
            var current = allDepts.FirstOrDefault(s => s.Id == deptId);
            if (current == null) return new List<int>();
            return allDepts.Where(s => s.TreePath.StartsWith(current.TreePath)).Select(s => s.Id).ToList();
        }


        /// <summary>
        /// 获取所有部门
        /// </summary>
        /// <returns></returns>
        public static List<RDept> GetDepts()
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                return RedisLib.Config.RedisHelper.GetHashAll<RDept>(db, Tables.Dept);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<RDept> GetTreeDepts()
        {
            try
            {
                var depts = GetDepts();
                var ret = new List<RDept>();
                foreach (var source in depts.Where(s => s.TreeLevel == 1).OrderBy(s => s.SortIndex))
                {
                    CompeleteDeptTree(source, depts, ret);
                }
                return ret;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="dept"></param>
        public static void UpdateDept(RDept dept)
        {
            try
            {
                var db = RedisLib.Config.RedisDbs.AuditFrameDb();
                RedisLib.Config.RedisHelper.SetHash(db, RedisLib.Model.Frame.Tables.Dept, dept.Id.ToString(), dept);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
