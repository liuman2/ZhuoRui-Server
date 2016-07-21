using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public class HitInfoModel
    {
        public string Collect_ID { get; set; }

        public string Collect_Set_Key { get; set; }
    }
    public class HitStaticModel
    {

        public string HashTypeName { get; set; }

        public string Collect_ID { get; set; }

        public string HashTypeId { get; set; }

        public int RecordCount { get; set; }

        public string RecordCountIdList { get; set; }

        public int HitCount { get; set; }

        public string HitCountIdList { get; set; }
    }
    public class HitStaticDetailModel
    {
        public int page { get; set; }

        public int rows { get; set; }

        public string hashTypeId { get; set; }

        public string fileName { get; set; }

        public string fileVal { get; set; }

        public DateTime? operateStart { get; set; }

        public DateTime? operateEnd { get; set; }
    }
}
