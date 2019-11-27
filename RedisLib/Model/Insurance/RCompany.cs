using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Model.Insurance
{
    public class RCompany
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string SearchKey { get; set; }

        public bool Enable { get; set; }

        public int SortIndex { get; set; }

        public string Remark { get; set; }

        public int? FaId { get; set; }

        public int TreeLevel { get; set; }
    }
}
