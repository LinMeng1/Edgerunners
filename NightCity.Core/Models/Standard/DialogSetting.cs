using NightCity.Core.Services;
using Prism.Mvvm;

namespace NightCity.Core.Models.Standard
{
    public class DialogSetting : BindableBase
    {
        private ActionOptimizingService _actionService;
        public DialogSetting()
        {
            _actionService = new ActionOptimizingService();
        }

        #region 内置延迟
        private int internalDelay = 200;
        public int InternalDelay
        {
            get => internalDelay;
            set
            {
                SetProperty(ref internalDelay, value);
            }
        }
        #endregion

        #region 对话框类型
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

        #region 对话框回调类型
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

        #region 对话框打开状态
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

        #region 对话框通用信息
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

        private bool dialogOpenCache = false;

        public void Hide()
        {
            dialogOpenCache = false;
            _actionService.Debounce(3 * InternalDelay, null, HideSubmit);
        }
        public void HideSubmit()
        {
            if(!dialogOpenCache)
            {
                DialogOpen = false;
            }
        }
        public void HideImmediately()
        {
            DialogOpen = false;
        }
        public void Show()
        {
            dialogOpenCache = true;
            DialogOpen = true;
        }
    }
}
