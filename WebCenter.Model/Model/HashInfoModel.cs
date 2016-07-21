using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public class HashInfoModel
    {
        public int total { get; set; }
        public IList<Hash_File_Info> List = new List<Hash_File_Info>();
    }
    /// <summary>
    /// 样本文件列表
    /// </summary>
    [Serializable]
    public class HashInfoItem
    {
        public string File_Info_ID { get; set; }
        public int File_Size { get; set; }
        public string Type_Name { get; set; }
        public string File_Name { get; set; }
        public string Extent_Name { get; set; }
        public DateTime Upload_Time { get; set; }
        public DateTime Check_Time { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
    /// <summary>
    /// 样本文件是否存在
    /// </summary>
    public class FileExitInfo
    {
        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool IsExist = false;
        /// <summary>
        /// 文件物理路径
        /// </summary>
        public string SourcePath { get; set; }
    }
}
