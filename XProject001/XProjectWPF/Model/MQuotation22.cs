using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XProjectWPF.Model
{
    /// <summary>
    /// 报价单实体
    /// </summary>
    public class MQuotation22
    {
        /// <summary>
        /// 获取或设置报价单号
        /// </summary>
        public string QuotationNo { get; set; }
        /// <summary>
        /// 获取或设置报价日期
        /// </summary>
        public DateTime QuotationDate { get; set; }
        /// <summary>
        /// 获取或设置业务跟进人
        /// </summary>
        public string FollowMan { get; set; }
        /// <summary>
        /// 获取或设置产品型号
        /// </summary>
        public string ProductModel { get; set; }
        /// <summary>
        /// 获取或设置申请项目
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 获取或设置当前报价
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 获取或设置是否含税
        /// </summary>
        public bool IsTax { get; set; }
        /// <summary>
        /// 获取或设置报价类型
        /// </summary>
        public string QuotationType { get; set; }
        /// <summary>
        /// 获取或设置公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 获取或设置公司地址
        /// </summary>
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 获取或设置联系人
        /// </summary>
        public string ContactMan { get; set; }
        /// <summary>
        /// 获取或设置联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 获取或设置电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 获取或设置传真
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// 获取或设置备注信息
        /// </summary>
        public string Remark { get; set; }
    }
}
