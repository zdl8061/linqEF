/*----------------------------------------------------------------
 *  Copyright (C) 2016 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：VTemplateHandler
 *  所属项目：
 *  创建用户：张德良
 *  创建时间：2016/9/28 星期三 上午 10:52:46
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTemplate.Engine;

namespace ZDL.TxRazor
{
    public class VTemplateHandler : Txooo.Web.Htmx.HtmxHandler1
    {

        private object GetBlogArchive()
        {
            //示例里我们只随便构造一个对象.在实际项目里你可以从数据库获取博客日记的真正数据
            return new
            {
                title = "这只是一篇测试的博客日记",
                content = "你好,这只是用于测试VTemplate模板引擎所用的博客日记",
                author = "kingthy",
                time = DateTime.Now,
                comments = new object[]{
                    new {author = "张三", time = DateTime.Now.AddDays(-1), content = "沙发"},
                    new {author = "李四", time = DateTime.Now.AddDays(0), content = "顶楼主"},
                    new {author = "王五", time = DateTime.Now.AddDays(1), content = "板凳啊"}
                }
            };
        }
        public override void ParsePage()
        {
            TemplateDocument document = new TemplateDocument(this.ThisPageCode.ToString());

            document.Variables.SetValue("blogarchive", this.GetBlogArchive());



            document.Variables.SetValue("this", this);
            document.Variables.SetValue("users", new[] { new { Id = 1, Name = "张三" }, new { Id = 2, Name = "李四" }, new { Id = 3, Name = "王五" } });

            document.Render(Response.Output);


            this.ThisPageCode.Clear();
            base.ParsePage();
        }
    }

   
}
