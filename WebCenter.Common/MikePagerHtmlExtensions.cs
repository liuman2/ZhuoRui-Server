using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Common
{
    public static class MikePagerHtmlExtensions
    {
        #region MikePager 分页控件

        public static String MikePager<T>(this HtmlHelper html, PagedList<T> data)
        {
            return html.MikePager(data.PageIndex, data.PageSize, data.TotalCount);
        }

        public static String MikePager(this HtmlHelper html, int pageIndex, int pageSize, int totalCount)
        {
            var totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
            var start = (pageIndex - 5) >= 1 ? (pageIndex - 5) : 1;
            var end = (totalPage - start) > 10 ? start + 10 : totalPage;

            var vs = html.ViewContext.RouteData.Values;

            var queryString = html.ViewContext.HttpContext.Request.QueryString;
            foreach (string key in queryString.Keys)
                vs[key] = queryString[key];

            NameValueCollection formString = html.ViewContext.HttpContext.Request.Form;
            foreach (string key in formString.Keys)
                vs[key] = formString[key];

            vs.Remove("X-Requested-With");
            vs.Remove("X-HTTP-Method-Override");

            var builder = new StringBuilder();
            builder.AppendFormat("<div class=\"mike_mvc_pager\">");

            //vs["pageSize"] = data.PageSize;
            if (pageIndex > 1)
            {
                vs["pageIndex"] = 1;

                builder.Append("<span class=\"ui-state-default  ui-corner-all\">");
                builder.Append(html.ActionLink("|<", vs["action"].ToString(), vs));
                builder.Append("</span>");

                vs["pageIndex"] = pageIndex - 1;
                builder.Append("<span class=\"ui-state-default  ui-corner-all\">");
                builder.Append(html.ActionLink("<", vs["action"].ToString(), vs)); 
                builder.Append("</span>");
            }

            for (int i = start; i <= end; i++) //前后各显示5个数字页码
            {
                vs["pageIndex"] = i;

                if (i == pageIndex)
                {
                    builder.Append("<span class=\"thispagethis ui-state-default  ui-corner-all\">");
                    builder.Append(i);
                    builder.Append("</span>");
                }
                else
                {
                    builder.Append("<span class=\"ui-state-default  ui-corner-all\">");
                    builder.Append(html.ActionLink(i.ToString(CultureInfo.InvariantCulture), vs["action"].ToString(), vs));
                    builder.Append("</span>");
                }
            }

            if ((pageIndex * pageSize) < totalCount)
            {
                vs["pageIndex"] = pageIndex + 1;
                builder.Append("<span class=\"ui-state-default  ui-corner-all\">");
                builder.Append(html.ActionLink(">", vs["action"].ToString(), vs));
                builder.Append("</span>");

                vs["pageIndex"] = totalPage;
                builder.Append("<span class=\"ui-state-default  ui-corner-all \">");
                builder.Append(html.ActionLink(">|", vs["action"].ToString(), vs));
                builder.Append("</span>");
            }
            
            var url = new UrlHelper(html.ViewContext.RequestContext);
            vs.Remove("pageIndex");
            builder.Append("<span>");
            builder.Append("<form action=\"" + url.Action(vs["action"].ToString(), vs) +
                           "\"  id=\"form1\" method=\"post\">");
            builder.Append("每页" + pageSize + "条/共" + totalCount + "条 第");
            builder.Append("<input type=\"text\" id=\"pageIndex\" name=\"pageIndex\" size=\"3\" value=" + pageIndex +
                           "  style=\"font-size:10px;text-align:center;border:0;border-bottom:1px solid #ccc;\"/>");
            builder.Append("页/共" + totalPage + "页");
            builder.Append("</form>");
            builder.Append("</span>");
            builder.Append("</div>");
            return builder.ToString();
        }

        #endregion
    }
}