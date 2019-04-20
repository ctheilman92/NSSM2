using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    public class ModelBase
    {
        [Key]
        [Column(name: "ID")]
        public int Id { get; set; }

        [Display(Name = "Active")]
        [Column(name: "IS_ACTIVE")]
        public bool IsActive { get; set; }

        [Display(Name ="Deleted")]
        [Column(name: "IS_DELETED")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Date Created")]
        [Column(name: "CREATED_DATE")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Date Modified")]
        [Column(name: "MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }
    }
}