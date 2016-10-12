using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class PrintData
    {
        public int id { get; set; }

        public string print_type { get; set; }

        /// <summary>
        /// 档案号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string project { get; set; }

        public string customer_name { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string company_cn { get; set; }

        public string company_en { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string date { get; set; }

        public DateTime? date_transaction { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string payer { get; set; }
        /// <summary>
        /// 收款事由
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 其他事项
        /// </summary>
        public string others { get; set; }

        /// <summary>
        /// 订单名称
        /// </summary>
        public string ordername { get; set; }

        /// <summary>
        /// 商标类别
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 提交方式
        /// </summary>
        public string mode { get; set; }

        /// <summary>
        /// 入账信息
        /// </summary>
        public string pay_info { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public string pay_way { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public float? amount { get; set; }
        /// <summary>
        /// 已收金额
        /// </summary>
        public float? received { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public float? balance { get; set; }
        /// <summary>
        /// 附件数量
        /// </summary>
        public int attachments { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 审核
        /// </summary>
        public string auditor { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string saleman { get; set; }

        /// <summary>
        /// 会计
        /// </summary>
        public string accounter { get; set; }

        /// <summary>
        /// 写票人
        /// </summary>
        public string creator { get; set; }

        public string currency { get; set; }

        public float? rate { get; set; }
    }
}