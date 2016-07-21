using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashLib.Model
{
    public class ModelQuery : ParameterQuery
    {
        public string Name { get; set; }

        public string RequestHttpType { get; set; }

        public string Role_Name { get; set; }

        public string Role_Code { get; set; }

    }
}
