using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RedisLib.Model.Common;
using StackExchange.Redis;
using RedisLib.Model.Insurance;

namespace RedisLib
{
    public class InsuranceHelp
    {
        private static IDatabase db => RedisLib.Config.RedisDbs.InsuranceDb();


        /// <summary>
        /// 删除日常Key
        /// </summary>
        public static void DeleteDailyKeys()
        {
            try
            {               
                RedisLib.Config.RedisHelper.DeleteKeys(db,
                    new List<string>()
                    {
                        Tabels.UserInfo.ToString(),
                        Tabels.Menus.ToString(),
                    });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 登陆用户
        /// </summary>
        /// <param name="userInfo"></param>
        public static void UserLogin(string uId,RInsuranceUserInfo userInfo)
        {
            try
            {
                RedisLib.Config.RedisHelper.SetHash(db, Tabels.UserInfo, uId, userInfo);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 获取登陆信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static RInsuranceUserInfo GetLogInfo(string guid)
        {
            try
            {
                return RedisLib.Config.RedisHelper.GetHashItem<RInsuranceUserInfo>(db, Tabels.UserInfo, guid);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 获取已登录用户
        /// </summary>
        /// <returns></returns>
        public static long GetLogInfoCounts()
        {
            try
            {
                return db.HashLength(Tabels.UserInfo.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


        /// <summary>
        /// 重Load所有保险公司
        /// </summary>
        /// <param name="companies"></param>
        public static void ReloadComapnies(List<RCompany> companies)
        {
            try
            {
                var hashItems = companies.Select(s => new HashEntry(s.Id, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                    .ToArray();
                db.HashSet(Tabels.Company.ToString(), hashItems);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 获取所有保险公司
        /// </summary>
        /// <returns></returns>
        public static List<RCompany> GetCompanies()
        {
            try
            {
                var items = RedisLib.Config.RedisHelper.GetHashAll<RCompany>(db, Tabels.Company);
                //保险公司默认2级
                var ret = new List<RCompany>();
                foreach (var fa in items.Where(s=>s.FaId==null).OrderBy(s=>s.SortIndex))
                {
                    ret.Add(fa);
                    ret.AddRange(items.Where(s=>s.FaId==fa.Id).OrderBy(s=>s.SortIndex));
                }
                return ret;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// 更新单个保险公司
        /// </summary>
        /// <param name="company"></param>
        public static void UpdateCompany(RCompany company)
        {
            try
            {
                RedisLib.Config.RedisHelper.SetHash(db, Tabels.Company, company.Id.ToString(), company);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Load所有角色权限
        /// </summary>
        /// <param name="items"></param>
        public static void ReloadRolePower(List<RRolePower> items)
        {
            try
            {
                var hashItems = items.Select(s => new HashEntry(s.RoleId, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                    .ToArray();
                db.HashSet(Tabels.RolePower.ToString(), hashItems);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #region 菜单              
        /// <summary>
        /// Load所有菜单
        /// </summary>
        /// <param name="items"></param>
        public static void ReLoadMenus(List<RMenus> items)
        {
            try
            {                              
                var hashItems = items.Select(s => new HashEntry(s.Id, Newtonsoft.Json.JsonConvert.SerializeObject(s)))
                    .ToArray();
                db.HashSet(Tabels.Menus.ToString(), hashItems);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
       

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns></returns>
        public static List<RMenus> GetMenus(RedisLib.Model.Insurance.RInsuranceUserInfo loginUser)
        {
            try
            {
                /*
                var items = RedisLib.Config.RedisHelper.GetHashAll<RMenus>(db, Tabels.Menus);
                //菜单永远2级
                var treeItems = new List<RMenus>();
                foreach (var item in items.Where(s => s.ParentID == null).OrderBy(s => s.SortIndex))
                {
                    treeItems.Add(item);
                    treeItems.AddRange(items.Where(s => s.ParentID == item.Id).OrderBy(s => s.SortIndex));
                }
                return treeItems;
                */
                //web后端绑定树
                return RedisLib.Config.RedisHelper.GetHashAll<RMenus>(db, Tabels.Menus);
            }
            catch (Exception e)
            {
                throw  new  Exception(e.Message);
            }
        }

        #endregion



        #region 权限相关

        /// <summary>
        /// 获取权限相关部门
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public static List<RedisLib.Model.Frame.RDept> GetPowerDepts(string guid,string head)
        {
            try
            {
                var ret = new List<RedisLib.Model.Frame.RDept>()
                {
                    new RedisLib.Model.Frame.RDept()
                    {
                        Id=0,
                        Name = head,
                        TreeLevel = 0,
                        TreePath = "0",
                        SortIndex = 0
                    }
                };
                var userInfo = GetLogInfo(guid);
                if (userInfo == null) return ret;
                
                //全部数据
                if (userInfo.Poser == 0)
                {
                    var treeDepts = RedisLib.FrameHelp.GetTreeDepts();
                    ret.AddRange(treeDepts);
                    return ret;
                }
                var allDepts = RedisLib.FrameHelp.GetDepts();
                var selfDept = allDepts.FirstOrDefault(s => s.Id == (userInfo?.DeptId ?? 0));
                if (selfDept != null)
                {
                    switch (userInfo.Poser)
                    {
                        case 1: //所在部门
                            selfDept.TreeLevel = 1;
                            ret.Add(selfDept);
                            break;
                        case 2: //所在部门及下级                         
                            var son = new List<RedisLib.Model.Frame.RDept>();
                            FrameHelp.CompeleteDeptTree(selfDept,allDepts, son);
                            var balanceLevel = selfDept.TreeLevel - 1;
                            if (balanceLevel > 0)
                            {
                                son.ForEach(s => s.TreeLevel = s.TreeLevel - balanceLevel);
                            }
                            ret.AddRange(son);
                            break;                      
                        default:
                            break;
                    }
                }
                return ret;
            }
            catch (Exception e)
            {
                 throw new  Exception(e.Message);
            }
        }


        /// <summary>
        /// 获取权限下所有部门Ids
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static List<int> CachePowerDeptIds(string guid)
        {
            var userInfo = GetLogInfo(guid);
            if (userInfo == null) return new List<int>();
            //全部数据
            if (userInfo.Poser == 0) return new List<int>(){-1};
            if(!userInfo.DeptId.HasValue) return new List<int>();
            //所在部门
            if(userInfo.Poser==1) return new List<int>(){userInfo.DeptId.Value};
            //所在部门及下级
            if (userInfo.Poser == 2)
            {
                return FrameHelp.CacheDeptChildren(userInfo.DeptId.Value);
            }
            //自己
            return new List<int>();
        }


        /// <summary>
        /// 根据下拉查询部门ids
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="selectDeptId"></param>
        /// <returns></returns>
        public static List<int> CachePowerDeptIds(string guid, int selectDeptId)
        {
            if (selectDeptId < 1) return CachePowerDeptIds(guid);
            var userInfo = GetLogInfo(guid);
            if(userInfo==null) return new List<int>(){selectDeptId};
            //全部或含下级权限
            if (userInfo.Poser == 0||userInfo.Poser==2)
            {
                return FrameHelp.CacheDeptChildren(selectDeptId);
            }
            return new List<int>() { selectDeptId };
        }

        #endregion       
    }
}
