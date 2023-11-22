using Prism.Mvvm;

namespace OnCall.Models.Standard
{
    public class AssignInfo : BindableBase
    {
        public string Cluster { get; set; }
        public string Category { get; set; }

        private string owner;
        public string Owner
        {
            get => owner;
            set => SetProperty(ref owner, value);
        }

        public string Creator { get; set; }

        private Users ownerInfo;
        public Users OwnerInfo
        {
            get => ownerInfo;
            set => SetProperty(ref ownerInfo, value);
        }

        private Users ownerInfoDisplay;
        public Users OwnerInfoDisplay
        {
            get => ownerInfoDisplay;
            set => SetProperty(ref ownerInfoDisplay, value);
        }

        private Users creatorInfo;
        public Users CreatorInfo
        {
            get => creatorInfo;
            set => SetProperty(ref creatorInfo, value);
        }

        private bool isVisible;
        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }
        public bool IsControllable { get; set; }       
    }
}
