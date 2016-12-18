﻿using System;
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
    }
}