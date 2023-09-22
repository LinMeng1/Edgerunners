using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightCity.Core.Models
{
    public class Connection_GetClusterOwners_Result
    {
        public string OwnerEmployeeId { get; set; }
        public string Owner { get; set; }
        public string Contact { get; set; }
        public string Organization { get; set; }
        public string Leader { get; set; }
        public string LeaderContact { get; set; }
    }
}
