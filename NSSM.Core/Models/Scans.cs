using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSSM.Core.Models
{
    [Table(name: "NS_SCAN_JOBS")]
    public class Scan : ModelBase
    {

        [Column(name: "STATUS")]
        public ScanStatus Status { get; set; }

        [Required]
        [Column(name: "NODE_INSTANCE")]
        public int SchedulerId { get; set; }
        public Node NodeInstance { get; set; }

        [Column(name: "CREATED_BY")]
        public int CreatedById { get; set; }
        public Member CreatedBy { get; set; }

        [Column(name: "EXPORT_PATH")]
        public string ExportPath { get; set; }

        [Column(name: "INVOKE_DATE")]
        public DateTime InvokeDate { get; set; }

        [Column(name: "END_DATE")]
        public DateTime EndDate { get; set; }

        [Column(name: "TIMEOUT")]
        public int Timeout { get; set; }

        [Column(name: "RETRY_ON_FAIL")]
        public bool RetryOnFail { get; set; }

        [Column(name: "ERROR")]
        public string Error { get; set; }

        [Column("PROJECT_INSTANCE")]
        public int ProjectId { get; set; }        
        public Project Project { get; set; }
    }

    public enum ScanStatus
    {
        None=0,
        Pending=1,
        Running=2,
        Error=3,
        Complete=4
    }
}