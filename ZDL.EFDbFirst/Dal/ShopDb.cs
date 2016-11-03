/*----------------------------------------------------------------
 *  Copyright (C) 2016 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：UserDb
 *  所属项目：
 *  创建用户：张德良
 *  创建时间：2016/9/26 星期一 上午 10:55:55
 *  
 *  功能描述：
 *          1、
 *          2、
 * 
 *  修改标识：
 *  修改描述：
 *  待 完 善：
 *          1、 
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using ZDL.EFDbFirst.Entity;

namespace ZDL.EFDbFirst.Dal
{
    public class ShopDb : BaseDAL
    {
        public override DbContext ctx
        {
            get
            {
                var _ctx = CallContext.GetData("ShopDb") as DbContext;
                if (_ctx == null)
                {
                    _ctx = new Entities();
                    CallContext.SetData("ShopDb", _ctx);
                }
                return _ctx;
            }
        }
    }
}
