using System.Text.RegularExpressions;

namespace Common
{
    public static class StringExtensions
    {
        public static string RemoveHtml(this string strHtml)
        {
            var regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            strHtml = regex.Replace(strHtml, "");
            strHtml = strHtml.Replace("&nbsp;", "");
            return strHtml;
        }

        public static string RemoveHtmlStyle(this string strHtml)
        {
            var regex = new Regex(@"\s[^<>]*>");
            strHtml = regex.Replace(strHtml, ">");

            regex = new Regex("<img.+?>");
            strHtml = regex.Replace(strHtml, "");


            return strHtml;
        }
    }
}
