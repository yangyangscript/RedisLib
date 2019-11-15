using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Model.Insurance
{
    public class RNotice
    {
        public int Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime NoticeTime { get; set; }

        public string Title { get; set; }

        public string Context { get; set; }

        public List<int> DeptIds { get; set; }
    }
}
