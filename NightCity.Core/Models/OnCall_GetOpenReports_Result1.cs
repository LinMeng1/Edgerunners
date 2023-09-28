using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightCity.Core.Models
{
    public class OnCall_GetOpenReports_Result1
    {
        public string Cluster { get; set; }
        public string ClusterCategory { get; set; }
        public List<OnCall_GetOpenReports_Result2> Repairs { get; set; }
    }
}
