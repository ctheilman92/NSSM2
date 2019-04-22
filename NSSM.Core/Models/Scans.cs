using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table(name: "NS_SCANS")]
    public class Scan : ModelBase
    {
        [Column(name: "SCAN_ALIAS")]
        public String ScanAlias { get; set; }

        [Column(name: "STATUS")]
        public ScanStatus Status { get; set; }

        [Column(name: "TARGET_URL")]
        public string TargetUrl { get; set; }

        [Column(name: "EXPORT_PATH")]
        public string ExportPath { get; set; }

        [Column(name: "INVOKE_DATE")]
        public DateTime? InvokeDate { get; set; }

        [Column(name: "END_DATE")]
        public DateTime? EndDate { get; set; }

        [Column(name: "TIMEOUT")]
        public int? Timeout { get; set; }

        [Column(name: "RETRY_ON_FAIL")]
        public bool RetryOnFail { get; set; }

        [Column(name: "ERROR")]
        public string Error { get; set; }

        #region FOREIGN KEYS

        [Column(name: "NODE_ID")]
        public int? NodeInstanceId { get; set; }
        public virtual Node NodeInstance { get; set; }

        [Column("PROJECT_ID")]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        #endregion
    }

    public enum ScanStatus
    {
        None=0,
        Pending=1,
        Running=2,
        Error=3,
        Complete=4,
        Terminated=5
    }
}