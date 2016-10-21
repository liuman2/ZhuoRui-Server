﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter.Web
{
    public class OrderSearchRequest
    {
        public int index { get; set; }
        public int size { get; set; }
        public int? customer_id { get; set; }
        public int? status { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
    }

    public class TrademarkRequest : OrderSearchRequest
    {
        public string name { get; set; }
        public string applicant { get; set; }
    }

    public class LectureRequest : OrderSearchRequest
    {
        public string title { get; set; }
        public string form { get; set; }
    }

    public class LetterRequest
    {
        public int index { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
    }

    public class LeaveSearchRequest
    {
        public int index { get; set; }
        public int size { get; set; }
        public int? status { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
    }

    public class LeaveResponse
    {
        public int id { get; set; }
        public int? owner_id { get; set; }
        public string owner_name { get; set; }
        public int? type { get; set; }
        public string type_name { get; set; }
        public DateTime? date_start { get; set; }
        public DateTime? date_end { get; set; }
        public string reason { get; set; }
        public string memo { get; set; }
        public int? receiver_id { get; set; }
        public string receiver_name { get; set; }
        public string tel { get; set; }
        public int? auditor_id { get; set; }
        public string auditor_name { get; set; }
        public string audit_memo { get; set; }
        public DateTime? date_review { get; set; }
        public int? status { get; set; }
        public string status_name { get; set; }
        public DateTime? date_created { get; set; }

        public string department { get; set; }
        public string position { get; set; }
    }
}