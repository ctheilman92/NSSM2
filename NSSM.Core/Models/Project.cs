﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NSSM.Core.Models
{
    [Table("NS_PROJECTS")]
    public class Project : ModelBase
    {
        [Column(name: "PROJECT_NAME")]
        public string ProjectName { get; set; }

        [Column(name: "SUMMARY_LOCATION")]
        public string SummaryLocation { get; set; }

        [Column(name: "ADMIN")]
        public int ProjectAdminId  { get; set; }
        public Member ProjectAdmin { get; set; }

        ICollection<Scan> ProjectScans { get; set; }
    }
}