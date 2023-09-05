using System;

namespace NightCity.Core.Models
{
    public class Banner_GetMessages_Result
    {
        public string Id { get; set; }
        public string Urgency { get; set; }
        public int Priority { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public string LinkCommand { get; set; }
        public string LinkInfomation { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
