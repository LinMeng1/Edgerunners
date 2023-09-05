using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NightCity.Core;
using NightCity.Core.Events;
using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using NightCity.Core.Services;
using NightCity.Core.Services.Prism;
using Prism.Events;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TestEngAuthorization.Views
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private HttpService httpService;
        IEventAggregator e;
        IPropertyService propertyService;
        public Login(IEventAggregator e, IPropertyService propertyService)
        {
            InitializeComponent();
            this.e = e;
            this.propertyService = propertyService;
            httpService = new HttpService();
            Bitmap background = Properties.Resources.background;
            using (var memory = new MemoryStream())
            {
                background.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Backgorund.Source = bitmapImage;
            }
            UsernameText.Focus();
        }
        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void UsernameText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PasswordText.Focus();
        }
        private async void PasswordText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await LoginCheck();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await LoginCheck();
        }
        private async Task LoginCheck()
        {
            LoginLoading.IsOpen = true;            
            try
            {
                string username = UsernameText.Text;
                string password = PasswordText.Password;
                try
                {
                    ControllersResult result = JsonConvert.DeserializeObject<ControllersResult>(await httpService.Post("https://10.114.113.101/api/basic/authorize/GetToken", new { Username = username, Password = password }));
                    if (result.Result)
                    {
                        Global.Log($"[TestEngAuthorization]:[Login]:[LoginCheck]:login success");
                        Authorize_GetToken_Result content = JsonConvert.DeserializeObject<Authorize_GetToken_Result>(result.Content.ToString());
                        propertyService.SetProperty("TestEngAuthorizationInfo", content.Token);
                        propertyService.SetProperty("DisplayName", content.Name);
                        e.GetEvent<AuthorizationInfoChangedEvent>().Publish(new Tuple<string, string>("TestEngAuthorization", "TestEngAuthorizationInfo"));
                        DialogResult = true;
                        Close();
                    }
                    else
                        ErrorTextHint.Text = result.ErrorMessage.ToString();
                }
                catch (Exception e)
                {
                    Global.Log($"[TestEngAuthorization]:[Login]:[LoginCheck]:exception:{e.Message}", true);
                    ErrorTextHint.Text = e.Message;
                }
            }
            catch (Exception ex)
            {
                Global.Log($"[TestEngAuthorization]:[MainViewModel]:[GetUserInfo]:exception:{ex.Message}", true);
            }
            LoginLoading.IsOpen = false;
            PasswordText.Focus();
        }
    }
}
