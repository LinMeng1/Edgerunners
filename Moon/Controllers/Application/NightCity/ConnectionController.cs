using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using Moon.Core.Standard;
using Microsoft.AspNetCore.Authorization;
using Moon.Attributes;
using Moon.Core.Utilities;

namespace Moon.Controllers.Application.NightCity
{
    [Route("api/application/night-city/connection/[action]")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly ILogger<ConnectionController> log;
        public ConnectionController(ILogger<ConnectionController> _log)
        {
            log = _log;
        }

        #region 获取集群
        [HttpPost]
        public ControllersResult GetClusters([FromBody] Connection_GetClusters_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<Connection_GetClusters_Result> clusters = Database.Edgerunners.Queryable<IPCClusters>().Where(it => it.Mainboard == parameter.Mainboard).Select(it => new Connection_GetClusters_Result()
                {
                    Category = it.Category,
                    Cluster = it.Cluster,
                }).ToList();
                result.Content = clusters;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 设置集群
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Connection_SetCluster)]
        [HttpPost]
        public ControllersResult SetCluster([FromBody] Connection_SetCluster_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string[] wildcard = new string[] { "/", "#", "+", "$" };
                if (parameter.Category != null)
                {
                    bool illegal = wildcard.Any(wildcard => parameter.Category.Contains(wildcard));
                    if (illegal)
                        throw new Exception("Category contains illegal character");
                }
                {
                    bool illegal = wildcard.Any(wildcard => parameter.Cluster.Contains(wildcard));
                    if (illegal)
                        throw new Exception("Cluster contains illegal character");
                }
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                IPCClusters clusters = new()
                {
                    Mainboard = parameter.Mainboard,
                    Category = parameter.Category,
                    Cluster = parameter.Cluster,
                    Creator = employeeID
                };
                Database.Edgerunners.Insertable(clusters).IgnoreColumns("CreateTime").ExecuteCommand();
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 删除集群
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Connection_RemoveCluster)]
        [HttpPost]
        public ControllersResult RemoveCluster([FromBody] Connection_RemoveCluster_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Database.Edgerunners.Deleteable<IPCClusters>().Where(it => it.Mainboard == parameter.Mainboard && it.Cluster == parameter.Cluster).ExecuteCommand();
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region 获取集群负责人
        [HttpPost]
        public ControllersResult GetClusterOwner([FromBody] Connection_GetClusterOwners_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Connection_GetClusterOwners_Result clusterOwner = new Connection_GetClusterOwners_Result();
                Users user = Database.Edgerunners.Queryable<IPCClusters_Owners, Users>((i, u) => i.Owner == u.EmployeeId && i.Cluster == parameter.Cluster && i.Category == parameter.Category).Select((i, u) => u).First();
                if (user == null) throw new Exception("Cant find cluster owner");
                clusterOwner.OwnerEmployeeId = user.EmployeeId;
                clusterOwner.Owner = user.Name;
                clusterOwner.Contact = user.Contact;
                Organizations org = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == user.Organization);
                if (org != null)
                {
                    clusterOwner.Organization = org.Name;
                    Users userLeader = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == org.Owner);
                    if (userLeader != null)
                    {
                        clusterOwner.Leader = userLeader.Name;
                        clusterOwner.LeaderContact = userLeader.Contact;
                    }
                }
                result.Content = clusterOwner;
                result.Result = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = $"Exception : {e.Message}";
                log.LogError(result.ErrorMessage);
            }
            return result;
        }
        #endregion

        #region GetClusters
        public class Connection_GetClusters_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class Connection_GetClusters_Result
        {
            public string? Category { get; set; }
            public string Cluster { get; set; }
        }
        #endregion

        #region SetCluster
        public class Connection_SetCluster_Parameter
        {
            public string Mainboard { get; set; }
            public string? Category { get; set; }
            public string Cluster { get; set; }
        }
        #endregion

        #region RemoveCluster
        public class Connection_RemoveCluster_Parameter
        {
            public string Mainboard { get; set; }
            public string Cluster { get; set; }
        }
        #endregion

        #region GetClusterOwner
        public class Connection_GetClusterOwners_Parameter
        {
            public string Cluster { get; set; }
            public string Category { get; set; }
        }
        public class Connection_GetClusterOwners_Result
        {
            public string OwnerEmployeeId { get; set; }
            public string Owner { get; set; }
            public string? Contact { get; set; }
            public string? Organization { get; set; }
            public string? Leader { get; set; }
            public string? LeaderContact { get; set; }
        }
        #endregion

    }
}
