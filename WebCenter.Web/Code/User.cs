using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCenter.Entities;

namespace WebCenter.Web
{
    public class Hex
    {
        public string ox { get; set; }
        public string water { get; set; }
        public string ph { get; set; }
    }
    public class User
    {
        public int id { get; set; }
        public int? organization_id { get; set; }
        public int? position_id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string english_name { get; set; }
        public string mobile { get; set; }
        public DateTime? birthday { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public string url { get; set; }
    }

    public class MailAddress
    {
        public int id { get; set; }
        public string receiver { get; set; }
        public string tel { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string mobile { get; set; }
        public string salesman { get; set; }
        public string source { get; set; }
        public int? source_id { get; set; }
        public string source_name { get; set; }
        public int? assistant_id { get; set; }
        public string assistant_name { get; set; }
        public string tel { get; set; }
        public string business_nature { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string QQ { get; set; }
        public string wechat { get; set; }
        public int? creator_id { get; set; }
        public int? salesman_id { get; set; }
        public int? organization_id { get; set; }
        public string description { get; set; }
        public string contacts { get; set; }
        public string code { get; set; }
        public string industry { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
        public string assistants { get; set; }

        public string mailling_address { get; set; }
        public string mailling_province { get; set; }
        public string mailling_city { get; set; }
        public string mailling_county { get; set; }

        public DateTime? date_created { get; set; }

        public List<Assistant> assistantList { get; set; }
        public List<Bank> banks { get; set; }

        public int business_count { get; set; }
    }

    public class Assistant
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Bank
    {
        public int id { get; set; }
        public int? customer_id { get; set; }
        public string bank { get; set; }
        public string holder { get; set; }
        public string account { get; set; }
    }

    public class simpleNotice
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime? created { get; set; }
        public bool isNew { get; set; }
    }

    public class AuditEntity
    {
        public int id { get; set; }
        public int? customer_id { get; set; }
        public string customer_name { get; set; }

        public string industry { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string customer_address { get; set; }
        public string contact { get; set; }
        public string mobile { get; set; }
        public string tel { get; set; }
        public string salesman { get; set; }
        public string accountant_name { get; set; }
        public string manager_name { get; set; }
        public string assistant_name { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public DateTime? date_setup { get; set; }
        public string address { get; set; }
        public string business_area { get; set; }
        public string trade_mode { get; set; }
        public int? has_parent { get; set; }
        public int? account_number { get; set; }
        public DateTime? account_period { get; set; }
        public string date_year_end { get; set; }
        public float? turnover { get; set; }
        public float? amount_bank { get; set; }
        public int? bill_number { get; set; }
        public string accounting_standard { get; set; }
        public float? cost_accounting { get; set; }
        public DateTime? date_transaction { get; set; }
        public float? amount_transaction { get; set; }
        public string currency { get; set; }
        public float? rate { get; set; }
        public string progress { get; set; }
        public int? status { get; set; }
        public int? finance_reviewer_id { get; set; }
        public DateTime? finance_review_date { get; set; }
        public string finance_review_moment { get; set; }
        public int? submit_reviewer_id { get; set; }
        public DateTime? submit_review_date { get; set; }
        public string submit_review_moment { get; set; }
        public int? review_status { get; set; }
        public DateTime? date_finish { get; set; }
        public int? creator_id { get; set; }
        public int? accountant_id { get; set; }
        public int? salesman_id { get; set; }
        public int? manager_id { get; set; }
        public int? organization_id { get; set; }
        public string description { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public int? annual_year { get; set; }
        public string turnover_currency { get; set; }
        public int? assistant_id { get; set; }
        public DateTime? account_period2 { get; set; }
        public string source { get; set; }
        public int? source_id { get; set; }
        public string source_code { get; set; }
    }

    public class SubAudit
    {
        public int id { get; set; }
        public int master_id { get; set; }
        public int? customer_id { get; set; }
        public DateTime? account_period { get; set; }
        public DateTime? account_period2 { get; set; }
        public string date_year_end { get; set; }
        public float? turnover { get; set; }
        public float? amount_bank { get; set; }
        public int? bill_number { get; set; }
        public string accounting_standard { get; set; }
        public float? cost_accounting { get; set; }
        public DateTime? date_transaction { get; set; }
        public float? amount_transaction { get; set; }
        public string currency { get; set; }
        public float? rate { get; set; }
        public string progress { get; set; }
        public int? status { get; set; }
        public int? finance_reviewer_id { get; set; }
        public DateTime? finance_review_date { get; set; }
        public string finance_review_moment { get; set; }
        public int? submit_reviewer_id { get; set; }
        public DateTime? submit_review_date { get; set; }
        public string submit_review_moment { get; set; }
        public int? review_status { get; set; }
        public DateTime? date_finish { get; set; }
        public int? creator_id { get; set; }
        public int? accountant_id { get; set; }
        public int? salesman_id { get; set; }
        public int? manager_id { get; set; }
        public string description { get; set; }
        public string turnover_currency { get; set; }
        public int? assistant_id { get; set; }
        public DateTime? date_created { get; set; }

        public string customer_name { get; set; }
        public string salesman { get; set; }
        public string accountant_name { get; set; }
        public string manager_name { get; set; }
    }

    public class LectureCustomer
    {
        public int id { get; set; }
        public int? lecture_id { get; set; }
        public int? customer_id { get; set; }
        public int? contact_id { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string mobile { get; set; }
        public string salesman { get; set; }
        public string source { get; set; }
        public int? source_id { get; set; }
        public string source_name { get; set; }
        public int? assistant_id { get; set; }
        public string assistant_name { get; set; }
        public string tel { get; set; }

        public string fax { get; set; }
        public string email { get; set; }
        public string QQ { get; set; }
        public string wechat { get; set; }
        public int? creator_id { get; set; }
        public int? salesman_id { get; set; }
        public int? organization_id { get; set; }
        public string description { get; set; }
        public string contacts { get; set; }
        public string code { get; set; }
        public string industry { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
        public int? status { get; set; }

    }

    public class InternalName
    {
        public string name { get; set; }
        public bool isFormal { get; set; }
    }

    public class InternalPrice
    {
        public string name { get; set; }
        public string memo { get; set; }
        public float price { get; set; }
    }

    public class Shareholder
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string cardNo { get; set; }
        public float takes { get; set; }
        public string memo { get; set; }
        public string type { get; set; }
        public string position { get; set; }
        public string changed_type { get; set; }
        public int? person_id { get; set; }
    }

    public class RegInternal : reg_internal
    {
        public List<InternalName> InternalNames { get; set; }
        public List<InternalPrice> InternalPrices { get; set; }
    }

    public class HistoryAbroad
    {
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public string address { get; set; }
        public string reg_no { get; set; }
        public string others { get; set; }
    }

    public class HistoryInternal
    {
        public string name_cn { get; set; }
        public string address { get; set; }
        public string reg_no { get; set; }
        public string legal { get; set; }
        public string director { get; set; }
        public string others { get; set; }
    }

    public class Contact
    {
        public string name { get; set; }
        public string customer_id { get; set; }
        //public string date_created { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string position { get; set; }
        public string QQ  { get; set; }
        public string tel { get; set; }
        //public string type { get; set; }
        public string wechat { get; set; }
    }

    public class Accounting
    {
        public int id { get; set; }
        public string code { get; set; }
        public int? customer_id { get; set; }
        public string customer_name { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string legal { get; set; }
        public int? status { get; set; }
        public int? review_status { get; set; }
        public DateTime? date_transaction { get; set; }
        public float? amount_transaction { get; set; }
        public int? amount_income { get; set; }
        public int? amount_unreceive { get; set; }
        public int? salesman_id { get; set; }
        public string salesman_name { get; set; }

        public int? assistant_id { get; set; }
        public string assistant_name { get; set; }

        public string finance_review_moment { get; set; }
        public string submit_review_moment { get; set; }
        public DateTime? date_created { get; set; }

        public DateTime? pay_notify { get; set; }

        public DateTime? date_start { get; set; }
        public DateTime? date_end { get; set; }
        public string period { get; set; }
    }

    public class AnnualExamEntity
    {
        public int id { get; set; }
        public string code { get; set; }
        public int? customer_id { get; set; }
        public string customer_code { get; set; }
        public string type { get; set; }
        public string customer_name { get; set; }
        public string name_cn { get; set; }
        public string name_en { get; set; }
        public sbyte?  status { get; set; }
        public int? review_status { get; set; }
        public DateTime? date_transaction { get; set; }
        public float? amount_transaction { get; set; }
        public int? amount_income { get; set; }
        public int? amount_unreceive { get; set; }
        public string progress { get; set; }
        public int? salesman_id { get; set; }
        public string salesman_name { get; set; }

        public int? assistant_id { get; set; }
        public string assistant_name { get; set; }

        public string finance_review_moment { get; set; }
        public string submit_review_moment { get; set; }
        public string receipt_no { get; set; }
    }

    public class LectureContact
    {
        public int id { get; set; }
        public int? customer_id { get; set; }
        public string name { get; set; }
        public string customer_name { get; set; }
        public int? customer_type { get; set; }
        public string mobile { get; set; }
        public string tel { get; set; }
        public string position { get; set; }
        public string email { get; set; }
        public string wechat { get; set; }
        public string QQ { get; set; }
        public string responsable { get; set; }
        public string industry { get; set; }
        public string business_nature { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
        public int? salesman_id { get; set; }
        public string salesman { get; set; }
        public int? status { get; set; }
    }

    public class LectureContactRequest
    {
        public int? contact_id { get; set; }
        public int? customer_id { get; set; }
    }

    public class LectureJoinMember
    {
        public int? lectureId { get; set; }
        public int? customerId { get; set; }
        public int? contactId { get; set; }
        public string customerName { get; set; }
        public string businessNature { get; set; }
        public string industry { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }
        public int? assistant_id { get; set; }
        public string assistants { get; set; }
        public int? salesman_id { get; set; }
        public string salesman { get; set; }
        public string contactName { get; set; }
        public string mobile { get; set; }
        public string tel { get; set; }
        public string email { get; set; }
        public string QQ { get; set; }
        public string wechat { get; set; }
    }

    public partial class TimeLine
    {
        public int id { get; set; }
        public int? source_id { get; set; }
        public string source_name { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime? date_created { get; set; }
        public DateTime? date_updated { get; set; }
        public DateTime? date_business { get; set; }
        public int? is_system { get; set; }
        public int? log_type { get; set; }
        public int? creator_id { get; set; }
    }
}
