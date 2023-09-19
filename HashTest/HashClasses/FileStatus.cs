using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace HasherTest.HashClasses
{
    public class FileStatus
    {
        [SetsRequiredMembers]
        public FileStatus(string fileName, SymbolRegular statusIcon, string statusMessage)
        {
            FileName = fileName;
            StatusIcon = statusIcon;
            StatusIconColor = GetStatusIconColor(statusIcon);
            StatusMessage = statusMessage;
        }


        public required string FileName { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public Brush StatusIconColor { get; set; }
        public SymbolRegular StatusIcon { get; set; }


        private Brush GetStatusIconColor(SymbolRegular statusIcon)
        {
            switch(statusIcon)
            {
                case SymbolRegular.CheckmarkCircle24:
                    return (new BrushConverter().ConvertFromString("#4CAF50") as Brush)!; //Green
                case SymbolRegular.DismissCircle24:
                    return (new BrushConverter().ConvertFromString("#F44336") as Brush)!; //Red
                default:
                    return (new BrushConverter().ConvertFromString("#FFFFFF") as Brush)!; //White
            }
        }

    }
}
