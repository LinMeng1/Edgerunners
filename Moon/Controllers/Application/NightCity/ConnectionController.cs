﻿using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Models;
using Moon.Core.Standard;
using Microsoft.AspNetCore.Authorization;
using Moon.Attributes;
using Moon.Core.Utilities;
using Moon.Core.Models._Imaginary;

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
                IPCClusters cluster = new()
                {
                    Mainboard = parameter.Mainboard,
                    Category = parameter.Category,
                    Cluster = parameter.Cluster,
                    Creator = employeeID
                };
                Database.Edgerunners.Insertable(cluster).IgnoreColumns("CreateTime").ExecuteCommand();
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

        #region 设置复数集群
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Connection_SetClusters)]
        [HttpPost]
        public ControllersResult SetClusters([FromBody] Connection_SetClusters_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                List<IPCClusters> clusters = new();
                string[] wildcard = new string[] { "/", "#", "+", "$" };
                foreach (var cluster in parameter.Clusters)
                {
                    if (cluster.Category != null)
                    {
                        bool illegal = wildcard.Any(wildcard => cluster.Category.Contains(wildcard));
                        if (illegal)
                            throw new Exception("Category contains illegal character");
                    }
                    {
                        bool illegal = wildcard.Any(wildcard => cluster.Cluster.Contains(wildcard));
                        if (illegal)
                            throw new Exception("Cluster contains illegal character");
                    }
                    clusters.Add(new()
                    {
                        Mainboard = parameter.Mainboard,
                        Category = cluster.Category,
                        Cluster = cluster.Cluster,
                        Creator = employeeID
                    });
                }
                Database.Edgerunners.Deleteable<IPCClusters>().Where(it => it.Mainboard == parameter.Mainboard).ExecuteCommand();
                Database.Edgerunners.Insertable(clusters).ExecuteCommand();
                try
                {
                    _ = Mqtt.Publish(parameter.Mainboard, new _MqttMessage()
                    {
                        IsMastermind = true,
                        Address = "Moon",
                        Sender = "Lucy",
                        Content = "system sync clusters"
                    });
                }
                catch { }
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

        #region 获取管辖权集群负责人
        [HttpPost]
        public ControllersResult GetJurisdictionalClusterOwner([FromBody] Connection_GetJurisdictionalClusterOwner_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Connection_GetJurisdictionalClusterOwner_Result jurisdictionalClusterOwner = new Connection_GetJurisdictionalClusterOwner_Result();
                Users user = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters, Users>((i, u) => i.Owner == u.EmployeeId && i.Mainboard == parameter.Mainboard).Select((i, u) => u).First();
                if (user == null) throw new Exception("Cant find cluster owner");
                jurisdictionalClusterOwner.OwnerEmployeeId = user.EmployeeId;
                jurisdictionalClusterOwner.Owner = user.Name;
                jurisdictionalClusterOwner.Contact = user.Contact;
                Organizations org = Database.Edgerunners.Queryable<Organizations>().First(it => it.Id == user.Organization);
                if (org != null)
                {
                    jurisdictionalClusterOwner.Organization = org.Name;
                    Users userLeader = Database.Edgerunners.Queryable<Users>().First(it => it.EmployeeId == org.Owner);
                    if (userLeader != null)
                    {
                        jurisdictionalClusterOwner.Leader = userLeader.Name;
                        jurisdictionalClusterOwner.LeaderContact = userLeader.Contact;
                    }
                }
                result.Content = jurisdictionalClusterOwner;
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

        #region 同步同辖管理集群消息
        [Authorize]
        [HttpPost]
        public ControllersResult SyncJurisdictionalClustersMessage([FromBody] Connection_SyncJurisdictionalClustersMessage_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<IPCs_JurisdictionalClusters> jurisdictionalClusters = Database.Edgerunners.Queryable<IPCs_JurisdictionalClusters>().RightJoin<IPCs_JurisdictionalClusters>((i, i2) => i.Owner == i2.Owner).Where((i, i2) => i.Mainboard == parameter.Mainboard).Select((i, i2) => i2).ToList();
                List<string> syncClusters = new();
                foreach (var cluster in jurisdictionalClusters)
                {
                    string syncCluster = $"{cluster.Category}/{cluster.Cluster}";
                    if (!syncClusters.Contains(syncCluster))
                        syncClusters.Add(syncCluster);
                }
                foreach (string syncCluster in syncClusters)
                {
                    try
                    {
                        _ = Mqtt.Publish(syncCluster, new _MqttMessage()
                        {
                            IsMastermind = true,
                            Address = "Moon",
                            Sender = "Lucy",
                            Content = "system sync banner messages"
                        });
                    }
                    catch { }
                }
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

        #region 设置集群负责人
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Connection_SetClusterOwner)]
        [HttpPost]
        public ControllersResult SetClusterOwner([FromBody] Connection_SetClusterOwner_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                IPCClusters_Owners clustersOwners = Database.Edgerunners.Queryable<IPCClusters_Owners>().First(it => it.Cluster == parameter.Cluster && it.Category == parameter.Category);
                if (clustersOwners != null)
                {
                    if (clustersOwners.Creator != employeeID)
                        throw new Exception("You cannot change a cluster created by someone else");
                    clustersOwners.Owner = parameter.Owner;
                    Database.Edgerunners.Updateable(clustersOwners).ExecuteCommand();
                }
                else
                {
                    clustersOwners = new IPCClusters_Owners()
                    {
                        Cluster = parameter.Cluster,
                        Category = parameter.Category,
                        Owner = parameter.Owner,
                        Creator = employeeID
                    };
                    Database.Edgerunners.Insertable(clustersOwners).ExecuteCommand();
                }
                try
                {
                    _ = Mqtt.Publish($"{parameter.Category}/{parameter.Cluster}", new Core.Models._Imaginary._MqttMessage()
                    {
                        IsMastermind = true,
                        Address = "Moon",
                        Sender = "Lucy",
                        Content = "system sync clusters"
                    });
                }
                catch { }
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

        #region 删除集群负责人
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_Connection_RemoveClusterOwner)]
        [HttpPost]
        public ControllersResult RemoveClusterOwner([FromBody] Connection_RemoveClusterOwner_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                string employeeID = Request.HttpContext.User.Claims.FirstOrDefault(it => it.Type == "EmployeeID").Value;
                IPCClusters_Owners clustersOwners = Database.Edgerunners.Queryable<IPCClusters_Owners>().First(it => it.Cluster == parameter.Cluster && it.Category == parameter.Category);
                if (clustersOwners != null)
                {
                    if (clustersOwners.Creator != employeeID)
                        throw new Exception("You cannot remove a cluster created by someone else");
                    Database.Edgerunners.Deleteable(clustersOwners).ExecuteCommand();
                }
                try
                {
                    _ = Mqtt.Publish($"{parameter.Category}/{parameter.Cluster}", new Core.Models._Imaginary._MqttMessage()
                    {
                        IsMastermind = true,
                        Address = "Moon",
                        Sender = "Lucy",
                        Content = "system sync clusters"
                    });
                }
                catch { }
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

        #region SetClusters
        public class Connection_SetClusters_Parameter
        {
            public string Mainboard { get; set; }
            public List<Connection_SetClusters_Parameter_Cluster> Clusters { get; set; }
        }
        public class Connection_SetClusters_Parameter_Cluster
        {
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

        #region GetJurisdictionalClusterOwner
        public class Connection_GetJurisdictionalClusterOwner_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class Connection_GetJurisdictionalClusterOwner_Result
        {
            public string OwnerEmployeeId { get; set; }
            public string Owner { get; set; }
            public string? Contact { get; set; }
            public string? Organization { get; set; }
            public string? Leader { get; set; }
            public string? LeaderContact { get; set; }
        }
        #endregion

        #region SyncJurisdictionalClustersMessage
        public class Connection_SyncJurisdictionalClustersMessage_Parameter
        {
            public string Mainboard { get; set; }
        }
        #endregion

        #region SetClusterOwner
        public class Connection_SetClusterOwner_Parameter
        {
            public string Cluster { get; set; }
            public string Category { get; set; }
            public string Owner { get; set; }
        }
        #endregion

        #region RemoveClusterOwner
        public class Connection_RemoveClusterOwner_Parameter
        {
            public string Cluster { get; set; }
            public string Category { get; set; }
        }
        #endregion

    }
}
