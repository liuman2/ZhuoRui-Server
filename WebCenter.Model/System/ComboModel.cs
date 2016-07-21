using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    /// <summary>
    /// 上传控件模型
    /// </summary>
    public class FileUploadModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// FTP服务器IP
        /// </summary>
        public string RemoteIP { get; set; }
        /// <summary>
        /// 获取当前用户的用户ID，当作密钥
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 主要用于判断MD5是否在Hash表中已存在
        /// </summary>
        public string ServiceUrl { get; set; }
        /// <summary>
        /// 最大文件传输大小，单位为M，默认为1024M
        /// </summary>
        public string MaxFileSize { get; set; }

    }
    /// <summary>
    /// 下拉列表模型
    /// </summary>
    public class ComboModel
    {
        public string id { get; set; }

        public string text { get; set; }

        public bool selected { get; set; }

    }

    /// <summary>
    /// checkbox模型
    /// </summary>
    public class CheckBoxModel
    {
        public string id { get; set; }

        public string text { get; set; }

        public bool Checked { get; set; }

    }
}
