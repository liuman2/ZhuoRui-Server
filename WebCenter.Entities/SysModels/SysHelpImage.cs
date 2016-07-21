using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.SysModels
{
    public class SysHelpImage : DbSetBase
    {
        [MaxLength(200)]
        public string Url { get; set; }

        [ForeignKey("SysHelp")]
        public Guid SysHelpId { get; set; }

        public virtual SysHelp SysHelp { get; set; }
    }
}