using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDL.LINQTest
{
    [TxTable(Name = "UserInfo", Base = "TxoooMy")]
    public class UserInfo
    {
        [Column(Name = "user_id", IsDbGenerated = true)]
        public int UserId { get; set; }

        [Column(Name = "username")]
        public string Username { get; set; }

        [Column(Name = "nickname")]
        public string Nickname { get; set; }

        [Column(Name = "ip")]
        public string Ip { get; set; }

        [Column(Name = "addtime", IsDbGenerated = true)]
        public DateTime AddTime { get; set; }
    }
}
