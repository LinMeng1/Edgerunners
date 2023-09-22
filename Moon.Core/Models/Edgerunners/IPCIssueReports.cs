using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("ipc-issue-reports")]
    public class IPCIssueReports
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Mainboard { get; set; }
        public string HostName { get; set; }
        public DateTime TriggerTime { get; set; }
        public string? InitialOwner { get; set; }
        public string? TransferOwner { get; set; }
        public DateTime ResponseTime { get; set; }
        public string? Responser { get; set; }
        public DateTime SolveTime { get; set; }
        public string? Solver { get; set; }
        public string State { get; set; }
        public string Product { get; set; }
        public string Process { get; set; }
        public string FailureCategory { get; set; }
        public string FailureReason { get; set; }
        public string Solution { get; set; }
        public string ExternalSystem { get; set; }
        public string ExternalId { get; set; }
    }
}
