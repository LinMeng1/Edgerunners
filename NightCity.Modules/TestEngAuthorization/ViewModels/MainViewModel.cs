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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TestEngAuthorization.Views;

namespace TestEngAuthorization.ViewModels
{
    public class MainViewModel : BindableBase, IDisposable, IAuthorizable
    {
        //内置延迟
        private readonly int internalDelay = 500;
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
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);
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
                DialogOpen = false;
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[GetUserInfoAsync]:exception:{ex.Message}", true);
                DialogMessage = ex.Message;
                DialogCategory = "Message";
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
                DialogOpen = true;
                DialogCategory = "Syncing";
                await Task.Delay(internalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Get("https://10.114.113.101/api/basic/account/GetRolesAndAuthorizations"));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                List<Account_GetRolesAndAuthorizations_Result> content = JsonConvert.DeserializeObject<List<Account_GetRolesAndAuthorizations_Result>>(result.Content.ToString());
                Roles = content;
                DialogOpen = false;
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[SyncRolesAndAuthorizationsAsync]:exception:{ex.Message}", true);
                DialogMessage = ex.Message;
                DialogCategory = "Message";
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
                DialogCategory = "Syncing";
                DialogOpen = true;
                await Task.Delay(internalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/account/SetContact", new { Contact = EditingContact }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                DialogOpen = false;
                Contact = EditingContact;
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[SetContactAsync]:exception:{ex.Message}", true);
                DialogMessage = ex.Message;
                DialogCategory = "Message";
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
                DialogCategory = "Syncing";
                DialogOpen = true;
                await Task.Delay(internalDelay);
                ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/account/SetPassword", new { OldPassword = EditingOldPassword, NewPassword = EditingNewPassword }));
                if (!result.Result)
                    throw new Exception(result.ErrorMessage);
                else
                {
                    DialogMessage = "The password has been changed, please log in again";
                    DialogCategory = "Message";
                    DialogCategoryCallback = "Logout";
                }
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[ChangePasswordAsync]:exception:{ex.Message}", true);
                DialogMessage = ex.Message;
                DialogCategory = "Message";
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
            DialogCategory = "Editing Contact";
            DialogCategoryCallback = "Editing Contact";
            DialogOpen = true;
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
            if (DialogCategoryCallback == "Logout")
                Disauthorize();
            else if (DialogCategoryCallback == null)
            {
                DialogOpen = false;
                DialogCategory = "Syncing";
            }
            else
                DialogCategory = DialogCategoryCallback;
            DialogMessage = string.Empty;
        }
        #endregion

        #region 命令：修改密码询问
        public ICommand TryChangePasswordCommand
        {
            get => new DelegateCommand(TryChangePassword);
        }
        private void TryChangePassword()
        {
            DialogCategory = "Change Password";
            DialogCategoryCallback = "Change Password";
            DialogOpen = true;
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
        public async void ChangePassword()
        {
            await ChangePasswordAsync();
        }
        #endregion

        #region 命令：取消操作
        public ICommand CancelCommand
        {
            get => new DelegateCommand(Cancel);
        }
        public void Cancel()
        {
            DialogCategoryCallback = null;
            DialogOpen = false;
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

        #region EmployeeId
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

        #region Name
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

        #region Position
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

        #region Email
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

        #region Contact
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

        #region Organization
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

        #region DialogOpen
        private bool dialogOpen = false;
        public bool DialogOpen
        {
            get => dialogOpen;
            set
            {
                SetProperty(ref dialogOpen, value);
            }
        }
        #endregion

        #region DialogCategory
        private string dialogCategory = "Syncing";
        public string DialogCategory
        {
            get => dialogCategory;
            set
            {
                SetProperty(ref dialogCategory, value);
            }
        }
        #endregion

        #region DialogCategoryCallback
        private string dialogCategoryCallback;
        public string DialogCategoryCallback
        {
            get => dialogCategoryCallback;
            set
            {
                SetProperty(ref dialogCategoryCallback, value);
            }
        }
        #endregion

        #region DialogMessage
        private string dialogMessage = string.Empty;
        public string DialogMessage
        {
            get => dialogMessage;
            set
            {
                SetProperty(ref dialogMessage, value);
            }
        }
        #endregion

        #region EditingContact
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

        #region Roles
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

        #region EditingOldPassword
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

        #region EditingNewPassword
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

        #region EditingNewPasswordAgain
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

        #endregion

        public void Dispose()
        {
            eventAggregator = null;
            propertyService = null;
            httpService = null;
        }
    }
}
