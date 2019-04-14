﻿using System;
using System.ComponentModel.DataAnnotations;

namespace NSSM.Core.Models
{
    public class ModelBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name ="Deleted")]
        public bool IsDeleted { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime ModifiedDate { get; set; }
    }
}