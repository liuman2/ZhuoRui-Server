using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public partial class Hash_File_Property
    {
        /// <summary>
        /// 文件上传ID
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 对应的文件MD5值（用在生成sqlite库文件，记录在Hash_File_Property表中）
        /// </summary>
        public string MD5 { get; set; }
    }
}
