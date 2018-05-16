using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RingRing
{
    class UserInfo
    {
        public UserInfo(bool login ,string Name, string PhoneNo)
        {
            UserInfo.Islogin = login;
            this.Name = Name;
            this.PhoneNo = PhoneNo;
        }

        public static bool Islogin { get; private set; }
        public string Name { get; }

        public string PhoneNo { get; }
    }
}
