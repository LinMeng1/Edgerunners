using SqlSugar;

namespace Moon.Core.Models
{
    public class Database
    {
        #region edgerunners
        private readonly static string EdgerunnersUid = "root";
        private readonly static string EdgerunnersPwd = "Xvke-7442";
        private readonly static string EdgerunnersServer = @"10.114.113.111";
        public readonly static SqlSugarScope Edgerunners = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = $@"server={EdgerunnersServer};database={"edgerunners"};uid={EdgerunnersUid};pwd={EdgerunnersPwd};Pooling=true;Max Pool Size=40000;Min Pool Size=0;SslMode=None",
            DbType = DbType.MySql,
            IsAutoCloseConnection = true
        });
        #endregion 
    }
}
