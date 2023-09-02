using Hasher.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hasher
{
    /// <summary>
    /// Interaction logic for HashFilesWindow.xaml
    /// </summary>
    public partial class HashFilesWindow
    {
        HashFilesViewModel _viewModel = new HashFilesViewModel();
        public HashFilesWindow()
        {
            this.DataContext = _viewModel;
            InitializeComponent();
        }
    }
}
