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

        private string ownerName;
        public string OwnerName
        {
            get => ownerName;
            set => SetProperty(ref ownerName, value);
        }

        private string secondmentOwner;
        public string SecondmentOwner
        {
            get => secondmentOwner;
            set => SetProperty(ref secondmentOwner, value);
        }

        private string secondmentOwnerName;
        public string SecondmentOwnerName
        {
            get => secondmentOwnerName;
            set => SetProperty(ref secondmentOwnerName, value);
        }

        private bool isVisible;
        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }
    }
}
