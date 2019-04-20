using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table(name: "NS_MEMBERS")]
    public class Member : ModelBase
    {
        [Required]
        [Column(name: "FIRST_NAME")]
        public string FirstName { get; set; }

        [Required]
        [Column(name: "LAST_NAME")]
        public string LastName { get; set; }

        [Required]
        [NotMapped]
        public Guid ADContextKey { get; set; }

        [Required]
        [Column(name: "EMAIL")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Column(name: "PHONE")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        #region FOREIGN KEYS

        public virtual ICollection<Scan> CreatedScans { get; set; }
        public virtual ICollection<Project> AdminProjects { get; set; }
        public virtual ICollection<Node> CreatedNodes { get; set; }
        public virtual ICollection<Node> AdminNodes { get; set; }

        #endregion
    }
}