using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Txooo;
using Txooo.Data;

namespace ZDL.LINQTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var user1 = new UserInfo { Username = "斯蒂芬", Nickname = "steven", Ip = "192.168.1.15" };
            var user2 = new UserInfo { Username = "分数段", Nickname = "yngwie", Ip = "192.168.1.13" };

            //TxData<UserInfo>.Insert(user1);
            //TxData<UserInfo>.Delete("Where Username=@Username AND IP=@IP ", "斯蒂芬", "192.168.1.15");
            //TxData<UserInfo>.Update("Username=@Username,IP=@IP WHERE nickname=@nickname", "asdf", "fefefe", "wangwu");
            //var userList = TxData<UserInfo>.Query("WHERE 1=1 order by user_id desc", pageSize: 2, currentPage: 2);

            var stateInfo = new StateInfo { Mname = "asdf", Fname = "owieu", AddTime = DateTime.Now, UserId = 21 };
            //TxData<StateInfo>.Insert(stateInfo);
            //TxData<StateInfo>.Delete("WHERE f_name=@f_name", "二分法");
            //TxData<StateInfo>.Update("f_name=@f_name WHERE m_name=@m_name", "测测测测", "违法");
            //var list = TxData<StateInfo>.Query("WHERE f_name=@f_name order by user_id desc", paramValue: new object[] { "owieu" });


            Console.ReadKey();
        }
    }
}
