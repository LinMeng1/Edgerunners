using Moon.Core.Utilities;
using SqlSugar;

namespace Moon.Core.Models
{
    public class Database
    {
        #region edgerunners
        private readonly static string EdgerunnersUid = "ixxLoMm8PKqg1rNi+9puWw==";
        private readonly static string EdgerunnersPwd = "XfeYZ67RsRGfhiGuVhrCag==";
        private readonly static string EdgerunnersServer = @"10.114.113.111";
        public readonly static SqlSugarScope Edgerunners = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = $@"server={EdgerunnersServer};database={"edgerunners"};uid={Encryption.DecryptString(EdgerunnersUid)};pwd={Encryption.DecryptString(EdgerunnersPwd)};Pooling=true;Max Pool Size=40000;Min Pool Size=0;SslMode=None",
            DbType = DbType.MySql,
            IsAutoCloseConnection = true
        });
        #endregion 
    }
}
