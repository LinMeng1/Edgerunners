using SqlSugar;

namespace Moon.Core.Models.Edgerunners
{
    [SugarTable("modules")]
    public class Modules
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Icon { get; set; }
        public string Author { get; set; }
        public bool IsOfficial { get; set; }
        public string Description { get; set; }
    }
}
