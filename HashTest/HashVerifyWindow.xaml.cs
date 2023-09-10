using HasherTest.ViewModels;
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

namespace HasherTest
{
    /// <summary>
    /// Interaction logic for HashVerifyWindow.xaml
    /// </summary>
    public partial class HashVerifyWindow 
    {
        HashVerifyViewModel _viewModel = new HashVerifyViewModel();
        public HashVerifyWindow()
        {
            this.DataContext = _viewModel;
            //FileList.ItemsSource = _viewModel.FileStatuses;
            InitializeComponent();
        }
    }
}
