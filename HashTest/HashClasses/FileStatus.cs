using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace HasherTest.HashClasses
{
    public class FileStatus
    {
        public required string FileName { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;
        public SymbolRegular StatusIcon { get; set; }
    }
}
