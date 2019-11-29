using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RedisLib.Model.Insurance
{
    public class RMenus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string NavigateUrl { get; set; }

        public int SortIndex { get; set; }

        public bool IsHid { get; set; }

        public int? ParentID { get; set; }

        public int? PowerId { get; set; }

        public string PowerName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public int TreeLevel {
            get { return this.ParentID.HasValue ? 1 : 0; }
        }
    }
}
