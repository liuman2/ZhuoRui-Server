using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class PrintData
    {
        public int id { get; set; }

        /// <summary>
        /// 档案号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string project { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string company { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string date { get; set; }

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
        /// 入账信息
        /// </summary>
        public string pay_info { get; set; }
        /// <summary>
        /// 收款方式
        /// </summary>
        public string pay_mode { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public double amount { get; set; }
        /// <summary>
        /// 已收金额
        /// </summary>
        public double received { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public double balance { get; set; }
        /// <summary>
        /// 附件数量
        /// </summary>
        public int attachments { get; set; }


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
    }
}