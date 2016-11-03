using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDL.LINQTest
{
    [TxTable(Name = "brand_apply", Base = "TxoooBrands")]
    public class BrandApply
    {
        #region 属性

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "brand_id")]
        public long BrandId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "brand_index_id")]
        public long BrandIndexId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "com_id")]
        public long ComId { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [Column(Name = "brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 所属主行业
        /// </summary>
        [Column(Name = "main_industry")]
        public string MainIndustry { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        [Column(Name = "establish_time")]
        public string EstablishTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "agent_time")]
        public string AgentTime { get; set; }

        /// <summary>
        /// 商标证书复印件
        /// </summary>
        [Column(Name = "brand_credential_img")]
        public string BrandCredentialImg { get; set; }

        /// <summary>
        /// 商标使用授权书
        /// </summary>
        [Column(Name = "brand_warrant_img")]
        public string BrandWarrantImg { get; set; }

        /// <summary>
        /// 品牌类型（0字号，1商标，2代理，3初创）
        /// </summary>
        [Column(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "add_user_id")]
        public int AddUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "add_time")]
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "check_user")]
        public string CheckUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "check_time")]
        public DateTime CheckTime { get; set; }

        /// <summary>
        /// 审核状态（0审核中，1通过审核，2未通过）
        /// </summary>
        [Column(Name = "check_state")]
        public string CheckState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Name = "remark")]
        public string Remark { get; set; }

        #endregion
    }
}
