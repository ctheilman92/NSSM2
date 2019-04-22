using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table(name: "NS_NODES")]
    public class Node : ModelBase
    {
        [Required]
        [Column(name: "ALIAS")]
        public string Alias { get; set; }

        [Column(name: "FQDN")]
        public string FQDN { get; set; }

        [Column(name: "DOMAIN")]
        public string Domain { get; set; }

        [Required]
        [Column(name: "NS_EXE_LOCATION")]
        public string ExecutableLocation { get; set; } = @"C:\Program Files (x86)\Netsparker";

        [Required]
        [Column(name: "CONCURRENT_SCANS")]
        public int Concurrentscans { get; set; } = 2;

        [Column(name: "START_TIME")]
        public DateTime? StartTime { get; set; }

        [Column(name: "STOP_TIME")]
        public DateTime? StopTime { get; set; }

        #region FOREIGN KEYS 

        [Column(name: "ADMIN_ID")]
        public int AdminMemberId { get; set; }
        [ForeignKey("AdminMemberId")]
        public virtual Member AdminMember { get; set; }

        [Column(name: "CREATED_BY")]
        public int CreatedbyId { get; set; }
        public virtual Member CreatedBy { get; set; }

        public virtual ICollection<Scan> Scans { get; set; }

        #endregion
    }
}