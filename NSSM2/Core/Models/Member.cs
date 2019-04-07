using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NSSM2.Core.Models
{
    [Table(name: "NS_MEMBERS")]
    public class Member
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

        [DataType(DataType.PhoneNumber)]
        [Column(name: "PHONE")]
        public string Phone { get; set; }

        #region EF FOREIGN KEYS

        ICollection<Scan> CreatedScans { get; set; }
        ICollection<Project> AdminProjects { get; set; }
        ICollection<Node> CreatedNodes { get; set; }
        #endregion
    }
}