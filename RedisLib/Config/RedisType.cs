using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Config
{
    public enum RedisType
    {     
        /// <summary>
        /// 一些常规的公共项目
        /// </summary>
        Common,

        /// <summary>
        /// 人员部门等基础结构
        /// </summary>
        AuditFrame,

        /// <summary>
        /// 车险独立系统
        /// </summary>
        Insurance,
    }
}
