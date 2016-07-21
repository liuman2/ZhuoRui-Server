using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public class CollectInfoCondition : ParameterQuery
    {
        public string Collect_Id { get; set; } 
        public string PlaceNam { get; set; }

        public string Collect_Set_Key { get; set; }
    }
}
