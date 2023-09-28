using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightCity.Core.Models
{
    public class OnCall_GetOpenReports_Result
    {
        public List<OnCall_GetOpenReports_Result2> LocalRepairs { get; set; }
        public List<OnCall_GetOpenReports_Result1> ClusterRepairs { get; set; }
    }
}
