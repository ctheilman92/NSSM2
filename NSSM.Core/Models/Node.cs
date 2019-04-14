using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table(name: "NS_NODE")]
    public class Node
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

        //other settings can be tracked and applied here


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