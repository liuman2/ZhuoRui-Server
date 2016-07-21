using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models.SysModels
{
    public class SysHelp : DbSetBase
    {
        public SysHelp()
        {
            Sort = 0;
            SysHelpImages = new List<SysHelpImage>();
        }

        [MaxLength(100)]
        [AdditionalMetadata("Size", 50)]
        [Required]
        public string Title { get; set; }

        [MaxLength]
        [DataType(DataType.Html)]
        [Required]
        [AllowHtml]
        public string Content { get; set; }

        [DataType("Files")]
        [Display(Name = "Image")]
        public string SysHelpImagesUrl { get; set; }

        public virtual ICollection<SysHelpImage> SysHelpImages { get; set; }

        public int Sort { get; set; }
    }
}