using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            StatusMessage = statusMessage;
        }
        public required string FileName { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public System.Windows.Media.Brush StatusIconColor { get; set; }
        public SymbolRegular StatusIcon { get; set; }
    }
}
