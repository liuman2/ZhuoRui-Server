using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class RegAbroad
    {
        public int ID { get; set; }
        public string 档案号 { get; set; }
        public string 公司中文名称 { get; set; }
        public string 公司英文名称 { get; set; }
        public int? 客户ID { get; set; }
        public string 客户名称 { get; set; }
        public string 注册地区 { get; set; }
        public string 公司董事 { get; set; }
        public string 公司股东 { get; set; }
        public DateTime? 成立日期 { get; set; }
        public string 注册编号 { get; set; }
        public string 注册地址 { get; set; }
        public sbyte? 是否开户 { get; set; }
        public DateTime? 成交日期 { get; set; }
        public float? 成交金额 { get; set; }
        public string 交易币别 { get; set; }
        public string 汇率 { get; set; }
        public string 业务员 { get; set; }
        public string 助理 { get; set; }
        public string 年检客服 { get; set; }
        public string 其他事项 { get; set; }
        public int? 订单归属人ID { get; set; }
        public string 订单归属人 { get; set; }
        public int? 客户归属ID { get; set; }
        public string 客户归属 { get; set; }        
        public string 订单状态 { get; set; }
        public int? order_status { get; set; }
        public int? status { get; set; }
    }

    public class ExcelLectureContact
    {
        public string 客户名称 { get; set; }
        public string 业务性质 { get; set; }
        public string 行业类别 { get; set; }
        public string 联系人 { get; set; }
        public string 手机 { get; set; }
        public string 座机 { get; set; }
        public string 邮箱 { get; set; }
        public string 省份 { get; set; }
        public string 城市 { get; set; }
        public string 地区 { get; set; }
        public string 地址 { get; set; }
        public string 业务员 { get; set; }
    }

    public class ExcelCustomerContact
    {
        public int ID { get; set; }
        public string 客户名称 { get; set; }
        public string 业务性质 { get; set; }
        public string 行业类别 { get; set; }
        public string 客户来源 { get; set; }
        //public string 介绍人 { get; set; }
        public string 省份 { get; set; }
        public string 城市 { get; set; }
        public string 地区 { get; set; }
        public string 地址 { get; set; }
        public string 业务员 { get; set; }
        public string 创建日期 { get; set; }
    }
}