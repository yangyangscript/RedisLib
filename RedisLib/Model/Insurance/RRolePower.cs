using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisLib.Model.Insurance
{
    public class RRolePower
    {
        public int RoleId { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string PowersStr { get; set; }

        public List<string> Powers {
            get
            {
                if(string.IsNullOrWhiteSpace(this.PowersStr)) return new List<string>();
                return this.PowersStr.Split(',').ToList();
            }
        }
    }  
}
