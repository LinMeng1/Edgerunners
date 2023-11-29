using Newtonsoft.Json;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Interfaces;
using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TestEngAuthorization.Views;

namespace TestEngAuthorization.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable, IAuthorizable
    {
        //事件聚合器
        private IEventAggregator eventAggregator;
        //属性服务
        private IPropertyService propertyService;
        //Http服务
        private HttpService httpService;
        public MainViewModel(IEventAggregator eventAggregator, IPropertyService propertyService)
        {
            //依赖注入及初始化
            this.eventAggregator = eventAggregator;
            this.propertyService = propertyService;
            httpService = new HttpService();
            //获取Token
            object TestEngAuthorizationInfo = propertyService.GetProperty("TestEngAuthorizationInfo");
            if (TestEngAuthorizationInfo == null)
            {
                Login login = new Login(eventAggregator, propertyService);
                var result = login.ShowDialog();
                if (result != true)
                {
                    eventAggregator.GetEvent<TemplateClosingEvent>().Publish("TestEngAuthorization");
                    return;
                }
                else
                    Token = propertyService.GetProperty("TestEngAuthorizationInfo").ToString();
            }
            else
                Token = TestEngAuthorizationInfo.ToString();
            Task.Run(async () =>
            {
                await SyncUserInfoAsync();
                await SyncRolesAndAuthorizationsAsync();
            });
        }

        /// <summary>
        /// 同步用户信息
        /// </summary>
        /// <returns></returns>
        private async Task SyncUserInfoAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/basic/account/GetUserInformation"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                Account_GetUserInformation_Result content = JsonConvert.DeserializeObject<Account_GetUserInformation_Result>(result.Content.ToString());
                EmployeeId = content.EmployeeId;
                ItCode = content.ItCode;
                Name = content.Name;
                Position = content.Position;
                Email = content.Email;
                Contact = content.Contact;
                Organization = content.Organization;
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null"); ;
                if (content.OfficeComputer == null)
                    LinkState = "No Link";
                else if (content.OfficeComputer == mainboard.ToString())
                    LinkState = "Linked";
                else
                    LinkState = "Link Change";
                MessageHost.Hide();
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[GetUserInfoAsync]:exception:{ex.Message}", true);
                MessageHost.DialogMessage = ex.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 同步角色和权限信息
        /// </summary>
        /// <returns></returns>
        private async Task SyncRolesAndAuthorizationsAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/basic/account/GetRolesAndAuthorizations"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<Account_GetRolesAndAuthorizations_Result> content = JsonConvert.DeserializeObject<List<Account_GetRolesAndAuthorizations_Result>>(result.Content.ToString());
                Roles = content;
                MessageHost.Hide();
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[SyncRolesAndAuthorizationsAsync]:exception:{ex.Message}", true);
                MessageHost.DialogMessage = ex.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 设置联系方式
        /// </summary>
        /// <returns></returns>
        private async Task SetContactAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/account/SetContact", new { Contact = EditingContact }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                MessageHost.Hide();
                Contact = EditingContact;
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[SetContactAsync]:exception:{ex.Message}", true);
                MessageHost.DialogMessage = ex.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        private async Task ChangePasswordAsync()
        {
            try
            {
                if (EditingNewPassword != EditingNewPasswordAgain)
                    throw new Exception("Your new and confirm password are different. Please enter your password again");
                if (EditingNewPassword == EditingOldPassword)
                    throw new Exception("Your old and new password are the same. Please enter your password again");
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/account/SetPassword", new { OldPassword = EditingOldPassword, NewPassword = EditingNewPassword }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                else
                {
                    MessageHost.DialogMessage = "The password has been changed, please log in again";
                    MessageHost.DialogCategory = "Message";
                    MessageHost.DialogCategoryCallback = "Logout";
                }
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[ChangePasswordAsync]:exception:{ex.Message}", true);
                MessageHost.DialogMessage = ex.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        /// <summary>
        /// 链接办公电脑
        /// </summary>
        /// <returns></returns>
        private async Task LinkOfficeComputerAsync()
        {
            try
            {
                MessageHost.Show();
                MessageHost.DialogCategory = "Syncing";
                await Task.Delay(MessageHost.InternalDelay);
                object mainboard = propertyService.GetProperty("Mainboard") ?? throw new Exception("Mainboard is null"); ;
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/account/LinkOfficeComputer", new { Mainboard = mainboard.ToString() }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                MessageHost.Hide();
                await SyncUserInfoAsync();             
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[LinkOfficeComputerAsync]:exception:{ex.Message}", true);
                MessageHost.DialogMessage = ex.Message;
                MessageHost.DialogCategory = "Message";
            }
        }

        #region 命令集合

        #region 命令：退出登录
        public ICommand LogoutCommand
        {
            get => new DelegateCommand(Disauthorize);
        }
        public void Disauthorize()
        {
            propertyService.SetProperty("TestEngAuthorizationInfo", null);
            propertyService.SetProperty("DisplayName", null);
            Global.Log($"[TestEngAuthorization]:[MainViewModel]:[Logout]:logout");
            eventAggregator.GetEvent<AuthorizationInfoChangedEvent>().Publish(new Tuple<string, string>("TestEngAuthorization", "TestEngAuthorizationInfo"));
            eventAggregator.GetEvent<TemplateClosingEvent>().Publish("TestEngAuthorization");
        }
        #endregion

        #region 命令：同步用户信息
        public ICommand SyncUserInfoCommand
        {
            get => new DelegateCommand(SyncUserInfo);
        }
        private async void SyncUserInfo()
        {
            await SyncUserInfoAsync();
        }

        #endregion

        #region 命令：同步角色和权限信息
        public ICommand SyncRolesAndAuthorizationsCommand
        {
            get => new DelegateCommand(SyncRolesAndAuthorizations);
        }
        private async void SyncRolesAndAuthorizations()
        {
            await SyncRolesAndAuthorizationsAsync();
        }
        #endregion

        #region 命令：设置联系方式询问
        public ICommand TrySetContactCommand
        {
            get => new DelegateCommand(TrySetContact);
        }
        private void TrySetContact()
        {
            MessageHost.DialogCategory = "Editing Contact";
            MessageHost.DialogCategoryCallback = "Editing Contact";
            MessageHost.Show();
            EditingContact = Contact;
        }
        #endregion

        #region 命令：设置联系方式
        public ICommand SetContactCommand
        {
            get => new DelegateCommand(SetContact);
        }
        private async void SetContact()
        {
            await SetContactAsync();
        }
        #endregion

        #region 命令：清除信息框
        public ICommand CleanMessageCommand
        {
            get => new DelegateCommand(CleanMessage);
        }
        private void CleanMessage()
        {
            if (MessageHost.DialogCategoryCallback == "Logout")
                Disauthorize();
            else if (MessageHost.DialogCategoryCallback == null)
            {
                MessageHost.HideImmediately();
                MessageHost.DialogCategory = "Syncing";
            }
            else
                MessageHost.DialogCategory = MessageHost.DialogCategoryCallback;
            MessageHost.DialogMessage = string.Empty;
        }
        #endregion

        #region 命令：修改密码询问
        public ICommand TryChangePasswordCommand
        {
            get => new DelegateCommand(TryChangePassword);
        }
        private void TryChangePassword()
        {
            MessageHost.DialogCategory = "Change Password";
            MessageHost.DialogCategoryCallback = "Change Password";
            MessageHost.Show();
            EditingOldPassword = string.Empty;
            EditingNewPassword = string.Empty;
            EditingNewPasswordAgain = string.Empty;
        }
        #endregion

        #region 命令：修改密码
        public ICommand ChangePasswordCommand
        {
            get => new DelegateCommand(ChangePassword);
        }
        private async void ChangePassword()
        {
            await ChangePasswordAsync();
        }
        #endregion

        #region 命令：取消操作
        public ICommand CancelCommand
        {
            get => new DelegateCommand(Cancel);
        }
        private void Cancel()
        {
            MessageHost.DialogCategoryCallback = null;
            MessageHost.HideImmediately();
        }
        #endregion

        #region 命令：链接办公电脑询问
        public ICommand TryLinkOfficeComputerCommand
        {
            get => new DelegateCommand(TryLinkOfficeComputer);
        }
        private async void TryLinkOfficeComputer()
        {
            await SyncUserInfoAsync();
            if (LinkState == "No Link")
                await LinkOfficeComputerAsync();
            else if (LinkState == "Link Change")
            {
                MessageHost.DialogMessage = "This account is already linked to another computer. Are you sure you want to change it?";
                MessageHost.DialogCategory = "MessageAsk";
                MessageHost.Show();
            }
        }
        #endregion

        #region 命令：链接办公电脑
        public ICommand LinkOfficeComputerCommand
        {
            get => new DelegateCommand(LinkOfficeComputer);
        }
        private async void LinkOfficeComputer()
        {
            await LinkOfficeComputerAsync();
        }
        #endregion

        #endregion

        #region 可视化属性集合

        #region Token
        private string token;
        public string Token
        {
            get => token;
            set
            {
                SetProperty(ref token, value);
                httpService.AddToken(value);
            }
        }

        #endregion

        #region 雇员编号
        private string employeeId;
        public string EmployeeId
        {
            get => employeeId;
            set
            {
                SetProperty(ref employeeId, value);
            }
        }
        #endregion

        #region 姓名
        private string name;
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
            }
        }
        #endregion

        #region ItCode
        private string itCode;
        public string ItCode
        {
            get => itCode;
            set
            {
                SetProperty(ref itCode, value);
            }
        }
        #endregion

        #region 职位
        private string position;
        public string Position
        {
            get => position;
            set
            {
                SetProperty(ref position, value);
            }
        }
        #endregion

        #region 电子邮箱
        private string email;
        public string Email
        {
            get => email;
            set
            {
                SetProperty(ref email, value);
            }
        }
        #endregion

        #region 联系方式
        private string contact;
        public string Contact
        {
            get => contact;
            set
            {
                SetProperty(ref contact, value);
            }
        }
        #endregion

        #region 链接状态
        private string linkState = "Link";
        public string LinkState
        {
            get => linkState;
            set
            {
                SetProperty(ref linkState, value);
            }
        }
        #endregion

        #region 组织
        private string organization;
        public string Organization
        {
            get => organization;
            set
            {
                SetProperty(ref organization, value);
            }
        }
        #endregion

        #region 编辑中联系方式
        private string editingContact;
        public string EditingContact
        {
            get => editingContact;
            set
            {
                SetProperty(ref editingContact, value);
            }
        }
        #endregion

        #region 权限角色
        private List<Account_GetRolesAndAuthorizations_Result> roles;
        public List<Account_GetRolesAndAuthorizations_Result> Roles
        {
            get => roles;
            set
            {
                SetProperty(ref roles, value);
            }
        }
        #endregion

        #region 编辑中旧密码
        private string editingOldPassword;
        public string EditingOldPassword
        {
            get => editingOldPassword;
            set
            {
                SetProperty(ref editingOldPassword, value);
            }
        }
        #endregion

        #region 编辑中新密码
        private string editingNewPassword;
        public string EditingNewPassword
        {
            get => editingNewPassword;
            set
            {
                SetProperty(ref editingNewPassword, value);
            }
        }
        #endregion

        #region 编辑中再次输入新密码
        private string editingNewPasswordAgain;
        public string EditingNewPasswordAgain
        {
            get => editingNewPasswordAgain;
            set
            {
                SetProperty(ref editingNewPasswordAgain, value);
            }
        }
        #endregion

        #region 对话框设置
        private DialogSetting messageHost = new DialogSetting();
        public DialogSetting MessageHost
        {
            get => messageHost;
            set
            {
                SetProperty(ref messageHost, value);
            }
        }
        #endregion

        #endregion

        public void Dispose()
        {
            eventAggregator = null;
            propertyService = null;
            httpService = null;
        }
    }
}
