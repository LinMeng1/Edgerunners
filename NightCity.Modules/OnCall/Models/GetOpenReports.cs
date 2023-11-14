using System;
using System.Collections.Generic;

namespace OnCall.Models
{
    public class GetOpenReports
    {
        public List<GetOpenReports_L2> LocalRepairs { get; set; }
        public List<GetOpenReports_L1> ClusterRepairs { get; set; }
    }
    public class GetOpenReports_L1
    {
        public string Cluster { get; set; }
        public string ClusterCategory { get; set; }
        public List<GetOpenReports_L2> Repairs { get; set; }
    }
    public class GetOpenReports_L2
    {
        public string Id { get; set; }
        public string Mainboard { get; set; }
        public string HostName { get; set; }
        public string State { get; set; }
        public DateTime TriggerTime { get; set; }
        public DateTime? ResponseTime { get; set; }
    }
}
