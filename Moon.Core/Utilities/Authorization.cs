namespace Moon.Core.Utilities
{
    public class Authorization
    {
        #region AuthorizationEnum
        public enum AuthorizationEnum
        {
            Default,//所有
            Application_MaxTac_User_AddUser,//新增用户
            Application_MaxTac_Organization_AddOrganization,//新增组织
            Application_MaxTac_Organization_SetOrganization,//修改组织
            Application_MaxTac_User_SetUser,//修改用户信息
            Application_MaxTac_Role_AddRole,//新增角色
            Application_MaxTac_Role_AddAuthorizationToRole,//为角色新增权限
            Application_MaxTac_User_AddRoleToUser,//为用户新增角色
            Application_MaxTac_User_DeleteUser,//删除用户
            Application_MaxTac_Project_AddProject,
            Application_MaxTac_Project_DeleteProject,
            Application_MaxTac_Role_DeleteAuthorizationToRole,//为角色删除权限
            Application_MaxTac_Project_AddIssueToProject,
            Application_MaxTac_Issue_AddMilestoneToIssue,
            Application_NightCity_ModuleManager_InstallModule,//安装模块
            Application_NightCity_ModuleManager_UninstallModule,//卸载模块
            Application_NightCity_Connection_SetCluster,//设置集群
            Application_NightCity_Connection_RemoveCluster,//删除集群
            Application_NightCity_Modules_OnCall_HandleReport,//处理OnCall报修信息
            Basic_Account_LinkOfficeComputer,//办公电脑链接
            Application_MaxTac_Publish_ReleaseProject,//发布项目
            Application_MaxTac_Organization_Secondment,//借调人员
            Application_NightCity_Connection_SetClusterOwner,//设置集群负责人
            Application_NightCity_Connection_RemoveClusterOwner,//删除集群负责人
            Application_MaxTac_Publish_UpdateNightCityModule,//更新NightCity模块
            Application_NightCity_Modules_TCN_TransformTCNOrder,//TCN转单
            Application_NightCity_Modules_OnCall_CallReportFromGigaeye,//从Gigaeye异常报修
            Application_NightCity_Banner_SetMessage,//设置置顶信息
            Application_NightCity_Connection_SetClusters,//设置复数集群
            Application_NightCity_ModuleManager_InstallModules,//安装/更新复数模块
        }
        #endregion
    }
}
