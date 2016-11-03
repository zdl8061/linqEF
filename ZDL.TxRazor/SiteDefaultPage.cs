
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Txooo.Web;
using Txooo.Web.Parse;

namespace ZDL.TxRazor
{
    public class SiteDefaultPage : Txooo.Web.Htmx.HtmxHandler1
    {
        protected dynamic Model = new ExpandoObject();

        public override void ParsePage()
        {
            ParseRazor(ref ThisPageCode);

            AddTempVariable("Brandname", "商机讲堂");
            AddTempVariable("Username", "open@txooo.com");
           
            base.ParsePage();
        }

        public virtual void ParseRazor(ref StringBuilder ThisPageCode)
        {
            var pageCode = ThisPageCode.ToString();

            string hash = ToMD5Hash(pageCode);

            Model.SiteName = "测试站点名称";
            Model.Name = "Steve Vai";

            Model.Username = "adfasdf";
            Model.Brandname = "商机讲堂"; 

            Model.UserList = new List<UserInfo>() { new UserInfo{ Username="列表数据1" }, new UserInfo { Username = "列表数据2" }, };


            pageCode = Razor.Parse(pageCode, Model);

            ThisPageCode.Clear().Append(pageCode);
        }

        public static string ToMD5Hash(string inputString)
        {
            System.Text.UnicodeEncoding sysEncode = new System.Text.UnicodeEncoding();
            Byte[] ByteToHash = sysEncode.GetBytes(inputString);
            Byte[] HashValue = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(ByteToHash);
            return System.BitConverter.ToString(HashValue);
        }

    }

    public class UserInfo
    {
        public string Username { get; set; }
    }

    /// <summary>
    /// 自定义razor原有模板，增加一些自定义的全局方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MyCustomTemplateBase<T> : TemplateBase<T>
    {
        public string MyUpper(string name)
        {
            return name.ToUpper();
        }
    }
}
