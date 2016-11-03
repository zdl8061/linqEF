/*----------------------------------------------------------------
 *  Copyright (C) 2016 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：GoodsAjax
 *  所属项目：
 *  创建用户：张德良
 *  创建时间：2016/9/30 星期五 上午 11:21:10
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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZDL.EFDbFirst.Dal;
using ZDL.EFDbFirst.Entity;

namespace ZDL.EFDbFirst.Ajax
{
    public class GoodsAjax : SiteAjaxBase
    {
        public string GetList(HttpContext context)
        {
            var _shopDal = new ShopDb();

            int _totalCount = 0;
            var _pageIndex = context.Request.QueryString["page"] ?? "1";
            var _pageSize = context.Request.QueryString["pagesize"] ?? "10";

            var _list = _shopDal.GetPagedList<Z_Goods, int>(int.Parse(_pageIndex), int.Parse(_pageSize), ref _totalCount, g => true, g => g.id, false);

            var _pages = (_totalCount + int.Parse(_pageSize) - 1) / int.Parse(_pageSize);

            var _json = JsonConvert.SerializeObject(new { count = _totalCount, pages = _pages, rows = _list });            

            return _json;
        }

        public string Add(HttpContext context)
        {
            var _barCode = context.Request.Params["barcode"];
            var _goodsType = context.Request.Form["goodsType"];
            var _name = context.Request.Params["name"];

            var _goods = new Z_Goods()
            {
                barcode = _barCode,
                goodsType = int.Parse(_goodsType),
                isAudit = false,
                isEnabled = false,
                name = _name
            };

            var _shopDal = new ShopDb();
            _goods = _shopDal.AddReturnModel(_goods);
            var _json = JsonConvert.SerializeObject(new { result = true, rows = _goods });

            return _json;
        }

        public string Delete(HttpContext context)
        {
            var _id = Convert.ToInt32(context.Request.QueryString["id"]);

            var _shopDal = new ShopDb();
            var _result = _shopDal.DelBy<Z_Goods>(goods => goods.id == _id);

            var _json = JsonConvert.SerializeObject(new { result = _result > 0 });
            return _json;
        }

        public string OnOffLine(HttpContext context)
        {
            var _id = Convert.ToInt32(context.Request.QueryString["id"]);
            var _isEnable = context.Request.QueryString["isEnabled"];

            var _model = new Z_Goods()
            {
                id = _id,
                isEnabled = bool.Parse(_isEnable)
            };

            var _shopDal = new ShopDb();
            var _result = _shopDal.Modify(_model, "isEnabled");

            var _json = JsonConvert.SerializeObject(new { result = _result > 0 });
            return _json;
        }
    }
}
