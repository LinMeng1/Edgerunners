namespace Moon.Core.Utilities
{
    public class Authorization
    {
        #region AuthorizationEnum
        public enum AuthorizationEnum
        {
            Default,
            Application_MaxTac_User_AddUser,
            Application_MaxTac_Organization_AddOrganization,
            Application_MaxTac_Organization_SetOrganization,
            Application_MaxTac_User_SetUser,
            Application_MaxTac_Role_AddRole,
            Application_MaxTac_Role_AddAuthorizationToRole,
            Application_MaxTac_User_AddRoleToUser,
            Application_MaxTac_User_DeleteUser,
            Application_MaxTac_Project_AddProject,
            Application_MaxTac_Project_DeleteProject,
            Application_MaxTac_Role_DeleteAuthorizationToRole,
            Application_MaxTac_Project_AddIssueToProject,
            Application_MaxTac_Issue_AddMilestoneToIssue,
            Application_NightCity_ModuleManager_InstallModule,
            Application_NightCity_ModuleManager_UninstallModule,
            Application_NightCity_Connection_SetCluster,
            Application_NightCity_Connection_RemoveCluster,
        }
        #endregion
    }
}
