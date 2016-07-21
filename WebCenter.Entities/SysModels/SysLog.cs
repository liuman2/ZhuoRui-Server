using System;
using System.ComponentModel.DataAnnotations;

namespace Models.SysModels
{
    public class SysLog : DbSetBase
    {
        public SysLog()
        {
          
        }

        [MaxLength(100)]
        public string Title { get; set; }
        
    }
}