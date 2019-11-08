using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLib.Model.Frame
{
    //SELECT Name,ID,Password FROM dbo.Users
    public class RUserLogin
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public string Password { get; set; }
    }
}
