using MaterialDesignThemes.Wpf;
using Prism.Mvvm;
using System.IO;
using System;

namespace NightCity.Core.Models.Standard
{
    public class Attachment : BindableBase
    {
        public PackIconKind Icon { get; set; }
        public string Name { get; set; }

        private string extension;
        public string Extension
        {
            get => extension;
            set
            {
                extension = value;
                switch (extension)
                {
                    case ".zip":
                        Icon = PackIconKind.FolderZip;
                        break;
                    case ".7z":
                        Icon = PackIconKind.FolderZip;
                        break;
                    case ".rar":
                        Icon = PackIconKind.FolderZip;
                        break;
                    case ".jpg":
                        Icon = PackIconKind.FileJpgBox;
                        break;
                    case ".png":
                        Icon = PackIconKind.FilePngBox;
                        break;
                    case ".gif":
                        Icon = PackIconKind.FileGifBox;
                        break;
                    case ".log_zip":
                        Icon = PackIconKind.AlphaNBox;
                        break;
                    case ".xlsx":
                        Icon = PackIconKind.MicrosoftExcel;
                        break;
                    case ".xls":
                        Icon = PackIconKind.MicrosoftExcel;
                        break;
                    case ".pptx":
                        Icon = PackIconKind.MicrosoftPowerpoint;
                        break;
                    case ".ppt":
                        Icon = PackIconKind.MicrosoftPowerpoint;
                        break;
                    case ".docx":
                        Icon = PackIconKind.MicrosoftWord;
                        break;
                    case ".doc":
                        Icon = PackIconKind.MicrosoftWord;
                        break;
                    case ".exe":
                        Icon = PackIconKind.BugPlay;
                        break;
                    default:
                        Icon = PackIconKind.FileQuestion;
                        break;
                }
            }
        }
        public string Directory { get; set; }
        public string SizeStr { get; set; }

        private long size;
        public long Size
        {
            get => size;
            set
            {
                size = value;
                if (size < 1024)
                    SizeStr = $"{size} Byte";
                else if (size < 1024 * 1024)
                    SizeStr = $"{(int)Math.Round((double)Size / 1024)} KB";
                else
                    SizeStr = $"{(int)Math.Round((double)Size / (1024 * 1024))} MB";
            }
        }
        public string Base64Str { get; set; }


    }
}
