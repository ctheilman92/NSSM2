﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table(name: "NS_NODE")]
    public class Node : ModelBase
    {
        [Required]
        [Column(name: "ALIAS")]
        public string Alias { get; set; }

        [Required]
        [Column(name: "FQDN")]
        public string FQDN { get; set; }

        [Required]
        [Column(name: "DOMAIN")]
        public string Domain { get; set; }

        [Required]
        [Column(name: "NS_EXE_LOCATION")]
        public string ExecutableLocation { get; set; }

        //other settings can be tracked and applied here
        [Required]
        [Column(name: "CONCURRENT_SCANS")]
        public int Concurrentscans { get; set; }

        [Column(name: "START_TIME")]
        public DateTime? StartTime { get; set; }

        [Column(name: "STOP_TIME")]
        public DateTime? StopTime { get; set; }

        #region FOREIGN KEYS 

        [Column(name: "ADMIN")]
        public int AdminMemberId { get; set; }
        [ForeignKey(name: "AdminMemberId")]
        public Member AdminMember { get; set; }

        [Column(name: "CREATED_BY")]
        public int CreatedbyId { get; set; }
        public Member CreatedBy { get; set; }

        public ICollection<Scan> Scans { get; set; }

        #endregion
    }
}