using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Attributes;
using Moon.Core.Models;
using Moon.Core.Models._Imaginary;
using Moon.Core.Models.Edgerunners;
using Moon.Core.Standard;
using Moon.Core.Utilities;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Moon.Controllers.Application.NightCity
{
    [Route("api/application/night-city/module-manager/[action]")]
    [ApiController]
    public class ModuleManagerController : ControllerBase
    {
        private readonly ILogger<ModuleManagerController> log;
        public ModuleManagerController(ILogger<ModuleManagerController> _log)
        {
            log = _log;
        }

        #region 获取模块
        [HttpPost]
        public ControllersResult GetModules([FromBody] ModuleManager_GetModules_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<ModuleManager_GetModules_Result> mods = Database.Edgerunners.Queryable<IPCs_Modules, Core.Models.Edgerunners.Modules, Users, Modules_Versions>((im, m, u, mv) => im.ModuleId == m.Id && im.IPCMainboard == parameter.Mainboard && m.Author == u.EmployeeId && mv.ModuleId == im.ModuleId && mv.Version == im.ModuleVersion).Select((im, m, u, mv) => new ModuleManager_GetModules_Result()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Category = m.Category,
                    Version = im.ModuleVersion,
                    Manifest = mv.Manifest,
                    Icon = m.Icon,
                    Author = u.Name,
                    AuthorItCode = u.ItCode,
                    IsOfficial = m.IsOfficial,
                    Description = m.Description,
                }).ToList();
                result.Content = mods;
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

        #region 获取模块版本信息
        [HttpPost]
        public ControllersResult GetModuleVersions([FromBody] ModuleManager_GetModuleVersions_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<ModuleManager_GetModuleVersions_Result> versions = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Modules_Versions>((m, mv) => m.Name == parameter.Module && m.Id == mv.ModuleId).Select((m, mv) => new ModuleManager_GetModuleVersions_Result
                {
                    Version = mv.Version,
                    ReleaseNotes = mv.ReleaseNotes,
                    Manifest = mv.Manifest,
                    ReleaseTime = mv.ReleaseTime,
                }).ToList();
                foreach (ModuleManager_GetModuleVersions_Result version in versions)
                {
                    if (version.Manifest == null) continue;
                    string json = version.Manifest;
                    if (json.StartsWith("["))
                    {
                        JArray jobj = JArray.Parse(json);
                        version.Manifest = jobj.ToString();
                    }
                    else if (json.StartsWith("{"))
                    {
                        JObject jobj = JObject.Parse(json);
                        version.Manifest = jobj.ToString();
                    }
                }
                result.Content = versions;
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

        #region 安装/更新模块
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_ModuleManager_InstallModule)]
        [HttpPost]
        public ControllersResult InstallModule([FromBody] ModuleManager_InstallModule_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Core.Models.Edgerunners.Modules mod = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Modules_Versions>((m, mv) => m.Id == mv.ModuleId && m.Name == parameter.ModuleName && mv.Version == parameter.ModuleVersion).First();
                if (mod == null)
                    throw new Exception($"Cant find module: {parameter.ModuleName} with version: {parameter.ModuleVersion}");
                IPCs_Modules ipcs_mods = new()
                {
                    IPCMainboard = parameter.Mainboard,
                    ModuleId = mod.Id,
                    ModuleVersion = parameter.ModuleVersion,
                };
                Database.Edgerunners.Storageable(ipcs_mods).ExecuteCommand();
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

        #region 安装/更新授权模块 
        [HttpPost]
        public ControllersResult InstallAuthorizeModule([FromBody] ModuleManager_InstallModule_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Core.Models.Edgerunners.Modules mod = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Modules_Versions>((m, mv) => m.Category == "Authorization" && m.Id == mv.ModuleId && m.Name == parameter.ModuleName && mv.Version == parameter.ModuleVersion).First();
                if (mod == null)
                    throw new Exception($"Cant find authorize module: {parameter.ModuleName} with version: {parameter.ModuleVersion}");
                IPCs_Modules ipcs_mods = new()
                {
                    IPCMainboard = parameter.Mainboard,
                    ModuleId = mod.Id,
                    ModuleVersion = parameter.ModuleVersion,
                };
                Database.Edgerunners.Storageable(ipcs_mods).ExecuteCommand();
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

        #region 获取所有模块
        [HttpGet]
        public ControllersResult GetAllModules()
        {
            ControllersResult result = new();
            try
            {
                List<ModuleManager_GetAllModules_Result> mods = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Users>((m, u) => m.Author == u.EmployeeId).Select((m, u) => new ModuleManager_GetAllModules_Result()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Category = m.Category,
                    Icon = m.Icon,
                    Author = u.Name,
                    AuthorItCode = u.ItCode,
                    IsOfficial = m.IsOfficial,
                    Description = m.Description,
                }).ToList();
                result.Content = mods;
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

        #region 获取所有模块及版本
        [HttpGet]
        public ControllersResult GetAllModulesWithVersions()
        {
            ControllersResult result = new();
            try
            {
                List<ModuleManager_GetAllModulesWithVersions_Result> mods = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Users>((m, u) => m.Author == u.EmployeeId).Select((m, u) => new ModuleManager_GetAllModulesWithVersions_Result()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Category = m.Category,
                    Icon = m.Icon,
                    Author = u.Name,
                    AuthorItCode = u.ItCode,
                    IsOfficial = m.IsOfficial,
                    Description = m.Description,
                }).ToList();
                List<Modules_Versions> versions = Database.Edgerunners.Queryable<Modules_Versions>().IgnoreColumns(it => it.Manifest).IgnoreColumns(it => it.ReleaseTime).OrderBy(it => it.Version).ToList();
                foreach (var mod in mods)
                {
                    foreach (var version in versions)
                    {
                        if (version.ModuleId == mod.Id)
                        {
                            mod.Version = version.Version;
                            mod.Versions.Add(new ModuleManager_GetAllModulesWithVersions_Result_Version()
                            {
                                Version = version.Version,
                                ReleaseNotes = version.ReleaseNotes
                            });
                        }
                    }
                }
                result.Content = mods;
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

        #region 安装/更新复数模块
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_ModuleManager_InstallModules)]
        [HttpPost]
        public ControllersResult InstallModules([FromBody] ModuleManager_InstallModules_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                List<IPCs_Modules> iPCs_Modules = new();
                foreach (var module in parameter.Modules)
                {
                    Core.Models.Edgerunners.Modules mod = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Modules_Versions>((m, mv) => m.Id == mv.ModuleId && m.Id == module.ModuleId && mv.Version == module.ModuleVersion).First();

                    Core.Models.Edgerunners.Modules modd = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules, Modules_Versions>((m, mv) => m.Id == mv.ModuleId && m.Id == module.ModuleId && mv.Version == module.ModuleVersion).First();
                    if (mod == null)
                        throw new Exception($"Cant find module: {module.ModuleId} with version: {module.ModuleVersion}");
                    iPCs_Modules.Add(new()
                    {
                        IPCMainboard = parameter.Mainboard,
                        ModuleId = mod.Id,
                        ModuleVersion = module.ModuleVersion,
                    });
                }
                Database.Edgerunners.Deleteable<IPCs_Modules>().Where(it => it.IPCMainboard == parameter.Mainboard).ExecuteCommand();
                Database.Edgerunners.Storageable(iPCs_Modules).ExecuteCommand();
                try
                {
                    _ = Mqtt.Publish(parameter.Mainboard, new _MqttMessage()
                    {
                        IsMastermind = true,
                        Address = "Moon",
                        Sender = "Lucy",
                        Content = "system reload"
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

        #region 卸载模块
        [Authorize]
        [PasswordCheck]
        [AuthorizeCheck(Authorization.AuthorizationEnum.Application_NightCity_ModuleManager_UninstallModule)]
        [HttpPost]
        public ControllersResult UninstallModule([FromBody] ModuleManager_UninstallModule_Parameter parameter)
        {
            ControllersResult result = new();
            try
            {
                Core.Models.Edgerunners.Modules mod = Database.Edgerunners.Queryable<Core.Models.Edgerunners.Modules>().First(it => it.Name == parameter.ModuleName);
                if (mod == null)
                    throw new Exception($"Cant find module: {parameter.ModuleName}");
                Database.Edgerunners.Deleteable<IPCs_Modules>().Where(it => it.IPCMainboard == parameter.Mainboard && it.ModuleId == mod.Id).ExecuteCommand();
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

        #region GetModules
        public class ModuleManager_GetModules_Parameter
        {
            public string Mainboard { get; set; }
        }
        public class ModuleManager_GetModules_Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Version { get; set; }
            public string Manifest { get; set; }
            public string Icon { get; set; }
            public string Author { get; set; }
            public string? AuthorItCode { get; set; }
            public bool IsOfficial { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region GetAllModules
        public class ModuleManager_GetAllModules_Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Icon { get; set; }
            public string Author { get; set; }
            public string? AuthorItCode { get; set; }
            public bool IsOfficial { get; set; }
            public string Description { get; set; }
        }
        #endregion

        #region GetAllModulesWithVersions
        public class ModuleManager_GetAllModulesWithVersions_Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Icon { get; set; }
            public string Author { get; set; }
            public string? AuthorItCode { get; set; }
            public bool IsOfficial { get; set; }
            public string Description { get; set; }
            public string Version { get; set; }
            public List<ModuleManager_GetAllModulesWithVersions_Result_Version> Versions { get; set; } = new();
        }
        public class ModuleManager_GetAllModulesWithVersions_Result_Version
        {
            public string Version { get; set; }
            public string ReleaseNotes { get; set; }
        }
        #endregion

        #region GetModuleVersions
        public class ModuleManager_GetModuleVersions_Parameter
        {
            public string Module { get; set; }
        }
        public class ModuleManager_GetModuleVersions_Result
        {
            public string Version { get; set; }
            public string ReleaseNotes { get; set; }
            public string Manifest { get; set; }
            public DateTime ReleaseTime { get; set; }
        }
        #endregion

        #region InstallModule
        public class ModuleManager_InstallModule_Parameter
        {
            public string Mainboard { get; set; }
            public string ModuleName { get; set; }
            public string ModuleVersion { get; set; }
        }
        #endregion

        #region InstallModules
        public class ModuleManager_InstallModules_Parameter
        {
            public string Mainboard { get; set; }
            public List<ModuleManager_InstallModules_Parameter_Module> Modules { get; set; }
        }
        public class ModuleManager_InstallModules_Parameter_Module
        {
            public int ModuleId { get; set; }
            public string ModuleVersion { get; set; }
        }
        #endregion

        #region UninstallModule
        public class ModuleManager_UninstallModule_Parameter
        {
            public string Mainboard { get; set; }
            public string ModuleName { get; set; }
        }
        #endregion
    }
}
