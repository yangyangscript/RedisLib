using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Model.Frame
{
    public class RUserInfo
    {
        public int Id { get; set; }

        public string ChineseName { get; set; }

        /// <summary>
        /// 0全部数据 1所在部门 2所在部门及下级 3自己
        /// </summary>
        public int UserPoser { get; set; }

        public int DeptId { get; set; }

        public string DeptName { get; set; }

        public int InsureRoleId { get; set; }
    }
}
