using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Models
{
    public interface IDbSetBase
    {
        Guid Id { get; set; }
        Guid EnterpriseId { get; set; }
        Guid UserId { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
        bool Deleted { get; set; }
    }

    public abstract class DbSetBase : IDbSetBase
    {
        protected DbSetBase()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
            Deleted = false;
        }

        [Key]
        [ScaffoldColumn(false)]
        public Guid Id { get; set; }

        [HiddenInput]
        public DateTime CreatedDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime UpdatedDate { get; set; }

        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }

        [ScaffoldColumn(false)]
        public Guid EnterpriseId { get; set; }

        [ScaffoldColumn(false)]
        public Guid UserId { get; set; }

        [ScaffoldColumn(false)]
        public bool Deleted { get; set; }
    }
}