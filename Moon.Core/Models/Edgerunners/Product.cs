using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("product")]
    public class Product
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Engineer { get; set; }
    }
}
