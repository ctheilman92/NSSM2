using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table("NS_PROJECTS")]
    public class Project : ModelBase
    {
        [Column(name: "PROJECT_NAME")]
        public string ProjectName { get; set; }

        [Column(name: "SUMMARY_LOCATION")]
        public string SummaryLocation { get; set; }

        #region FOREIGN KEYS

        [Column(name: "ADMIN_ID")]
        public int ProjectAdminId { get; set; }

        public virtual ICollection<Scan> ProjectScans { get; set; }

        #endregion
    }
}