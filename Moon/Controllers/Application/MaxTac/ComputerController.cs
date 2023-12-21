using Microsoft.AspNetCore.Mvc;
using Moon.Core.Models;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using Newtonsoft.Json;
using System.Reflection;

namespace Moon.Controllers.Application.MaxTac
{
    [Route("api/application/max-tac/computer/[action]")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        private readonly ILogger<ComputerController> log;
        public ComputerController(ILogger<ComputerController> _log)
        {
            log = _log;
        }

        #region 获取计算机列表
        [HttpPost]
        public ControllersResult GetComputerList([FromBody] Computer_GetComputerList_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                var query = Database.Edgerunners.Queryable<IPCs>();
                if (parameter.Mainboard != null && parameter.Mainboard != string.Empty)
                    query = query.Where(it => it.Mainboard.ToUpper().Contains(parameter.Mainboard.ToUpper()));
                if (parameter.HostName != null && parameter.HostName != string.Empty)
                    query = query.Where(it => it.HostName.ToUpper().Contains(parameter.HostName.ToUpper()));
                if (parameter.HostAddress != null && parameter.HostAddress != string.Empty)
                    query = query.Where(it => it.HostAddress.Contains(parameter.HostAddress));

                var queryCluster = Database.Edgerunners.Queryable<IPCClusters>();
                if (parameter.ClusterCategory != null && parameter.ClusterCategory != string.Empty)
                    queryCluster = queryCluster.Where(it => it.Category != null && it.Category.ToUpper().Contains(parameter.ClusterCategory.ToUpper()));
                if (parameter.Cluster != null && parameter.Cluster != string.Empty)
                    queryCluster = queryCluster.Where(it => it.Cluster != null && it.Cluster.ToUpper().Contains(parameter.Cluster.ToUpper()));
                List<string> mainboards = queryCluster.Select(it => it.Mainboard).ToList();

                if (parameter.Cluster != null && parameter.Cluster != string.Empty)
                    query = query.Where(it => mainboards.Contains(it.Mainboard));

                result.Content = query.ToList();
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

        #region 获取计算机基础信息
        [HttpPost]
        public ControllersResult GetComputerBasicInfomation([FromBody] Computer_GetComputerBasicInfomation_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Computer_GetComputerBasicInfomation_Result resultx = new();
                var Ipc = Database.Edgerunners.Queryable<IPCs>().First(it => it.Mainboard == parameter.Mainboard);
                if (Ipc == null) throw new Exception("Computer information not found");
                resultx.Mainboard = Ipc.Mainboard;
                resultx.HostName = Ipc.HostName;
                resultx.HostAddress = Ipc.HostAddress;
                resultx.HostMac = Ipc.HostMac;
                resultx.Domain = Ipc.Domain;
                resultx.Cpu = Ipc.Cpu;
                resultx.Disk = Ipc.Disk;
                resultx.Memory = Ipc.Memory;
                resultx.Linker = Database.Edgerunners.Queryable<Users_OfficePCs>().LeftJoin<Users>((uo, u) => uo.EmployeeId == u.EmployeeId).Where((uo, u) => uo.Mainboard == parameter.Mainboard).Select((uo, u) => u.Name).First();
                resultx.ConnectionHistories = Database.Edgerunners.Queryable<ConnectionHistories>().Where(it => it.Client == parameter.Mainboard).OrderBy(it => it.Time, SqlSugar.OrderByType.Desc).OrderBy(it => it.Id, SqlSugar.OrderByType.Desc).Take(20).Select(it => new Computer_GetComputerBasicInfomation_Result_ConnectionHistory
                {
                    IsConnected = it.IsConnected,
                    Time = it.Time,
                }).ToList();
                resultx.NightCityLaunchHistories = Database.Edgerunners.Queryable<NightCityLaunchHistories>().Where(it => it.Mainboard == parameter.Mainboard).OrderBy(it => it.Time, SqlSugar.OrderByType.Desc).Take(20).Select(it => new Computer_GetComputerBasicInfomation_Result_NightCityLaunchHistory
                {
                    Version = it.Version,
                    Time = it.Time
                }).ToList();
                resultx.UploadHistories = new();
                List<IPCHistories> iPCHistories = Database.Edgerunners.Queryable<IPCHistories>().Where(it => it.Mainboard == parameter.Mainboard && it.UploadTime > DateTime.Now.AddMonths(-3)).OrderBy(it => it.UploadTime, SqlSugar.OrderByType.Desc).ToList();
                for (int i = 0; i < iPCHistories.Count; i++)
                {
                    var last = resultx.UploadHistories.LastOrDefault();
                    if (last == null)
                    {
                        Computer_GetComputerBasicInfomation_Result_UploadHistory history = new()
                        {
                            Time = iPCHistories[i].UploadTime,
                            Changes = new()
                        };
                        resultx.UploadHistories.Add(history);
                    }
                    else
                    {
                        Type type = iPCHistories[i].GetType();
                        PropertyInfo[] properties = type.GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            if (property.Name == "Id" || property.Name == "UploadTime") continue;
                            object? value1 = property.GetValue(iPCHistories[i - 1]);
                            object? value2 = property.GetValue(iPCHistories[i]);
                            if (!Equals(value1, value2))
                            {
                                last.Changes.Add(new Computer_GetComputerBasicInfomation_Result_UploadHistory_Change
                                {
                                    Category = property.Name,
                                    Before = value2 == null ? string.Empty : value2.ToString(),
                                    After = value1 == null ? string.Empty : value1.ToString(),
                                });
                                Computer_GetComputerBasicInfomation_Result_UploadHistory history = new()
                                {
                                    Time = iPCHistories[i].UploadTime,
                                    Changes = new()
                                };
                                resultx.UploadHistories.Add(history);
                            }
                        }
                    }
                }
                result.Content = resultx;
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

        #region 获取幽灵客户端
        [HttpGet]
        public async Task<ControllersResult> GetMissingComputersAsync()
        {
            ControllersResult result = new();
            try
            {
                List<string> ipcs = Database.Edgerunners.Queryable<IPCs>().Where(it => it.IsConnected == 1).Select(it => it.Mainboard).ToList();
                List<EMQXAPIResult_Client> missingIpcs = new();
                List<string> missingIpcsx = new();
                int limit = 1000;
                int page = 1;
                string destination = $"http://10.114.113.101:18083/api/v5/clients?limit={limit}&page={page}";
                Dictionary<string, string> headers = new()
                {
                    { "Authorization", "Basic MTM1YTRmOWQ5OWI2ZWViZDp4dW1YRkJ6TklCNHBTbVlPNnNSWm1oRDJZUHNoTTZNdmxnZzVNdTFqQmRK" }
                };
                EMQXAPIResult clients = JsonConvert.DeserializeObject<EMQXAPIResult>(await Http.Get(destination, headers));
                while (clients.meta.count > limit * page)
                {
                    page++;
                    destination = $"http://10.114.113.101:18083/api/v5/clients?limit={limit}&page={page}";
                    EMQXAPIResult clientsx = JsonConvert.DeserializeObject<EMQXAPIResult>(await Http.Get(destination, headers));
                    clients.data.AddRange(clientsx.data);
                }
                foreach (var client in clients.data)
                {
                    if (!ipcs.Contains(client.clientid))
                        missingIpcs.Add(client);
                }
                foreach (var ipc in ipcs)
                {
                    if (clients.data.FirstOrDefault(it => it.clientid == ipc) == null)
                        missingIpcsx.Add(ipc);
                }
                result.Content = new { missingIpcs, missingIpcsx };
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

        #region GetComputerList
        public class Computer_GetComputerList_Parameter
        {
            public string? Mainboard { get; set; }
            public string? HostName { get; set; }
            public string? HostAddress { get; set; }
            public string? ClusterCategory { get; set; }
            public string? Cluster { get; set; }
        }
        #endregion

        #region GetComputerBasicInfomation
        public class Computer_GetComputerBasicInfomation_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class Computer_GetComputerBasicInfomation_Result
        {
            public string Mainboard { get; set; }
            public string HostName { get; set; }
            public string HostAddress { get; set; }
            public string HostMac { get; set; }
            public string Domain { get; set; }
            public string Cpu { get; set; }
            public string Disk { get; set; }
            public string Memory { get; set; }
            public string Linker { get; set; }
            public List<Computer_GetComputerBasicInfomation_Result_ConnectionHistory> ConnectionHistories { get; set; }
            public List<Computer_GetComputerBasicInfomation_Result_NightCityLaunchHistory> NightCityLaunchHistories { get; set; }
            public List<Computer_GetComputerBasicInfomation_Result_UploadHistory> UploadHistories { get; set; }
        }
        public class Computer_GetComputerBasicInfomation_Result_ConnectionHistory
        {
            public bool IsConnected { get; set; }
            public DateTime Time { get; set; }
        }
        public class Computer_GetComputerBasicInfomation_Result_NightCityLaunchHistory
        {
            public string Version { get; set; }
            public DateTime Time { get; set; }
        }
        public class Computer_GetComputerBasicInfomation_Result_UploadHistory
        {
            public List<Computer_GetComputerBasicInfomation_Result_UploadHistory_Change> Changes { get; set; }
            public DateTime Time { get; set; }
        }
        public class Computer_GetComputerBasicInfomation_Result_UploadHistory_Change
        {
            public string Category { get; set; }
            public string Before { get; set; }
            public string After { get; set; }
        }
        #endregion

        #region GetMissingComputers
        private class EMQXAPIResult
        {
            public List<EMQXAPIResult_Client> data { get; set; }
            public EMQXAPIResult_Meta meta { get; set; }
        }
        private class EMQXAPIResult_Client
        {
            public string clientid { get; set; }
            public string ip_address { get; set; }
            public bool connected { get; set; }
        }
        private class EMQXAPIResult_Meta
        {
            public int count { get; set; }
            public bool hasnext { get; set; }
            public int limit { get; set; }
            public int page { get; set; }
        }
        #endregion
    }
}
