using RJ.XStyle;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XProjectWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow :  XBaseForm 
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }

        private void t_btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void t_btn_Login_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            ManageForm myForm = new ManageForm();
            //Window1 myForm = new Window1();
            myForm.ShowDialog();
            this.Close();
        }
    }
}
