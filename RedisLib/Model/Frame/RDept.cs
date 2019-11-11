using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Model.Frame
{
    public class RDept
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TreeLevel { get; set; }
        public string TreePath { get; set; }
        public int SortIndex { get; set; }
    }
}
