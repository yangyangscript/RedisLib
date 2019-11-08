using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Model.Insurance
{
    public class RInsuranceUserInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ChineseName { get; set; }

        public int? RoleId { get; set; }

        public int? DeptId { get; set; }

        public string DeptName { get; set; }
        public int Poser { get; set; }
    }
}
