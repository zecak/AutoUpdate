using System;
using System.Collections.Generic;
using System.IO;
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

namespace Anye.Soft.AutoUpdate.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public async Task StartAsync(bool noUI = true)
        {
            var fullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoUpdate", "Anye.Soft.AutoUpdate.exe");
            if (File.Exists(fullName))
            {
                await Task.Run(() =>
                {
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = fullName;
                    p.StartInfo.CreateNoWindow = noUI;
                    p.Start();
                });

            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await StartAsync();//后台运行;注:AutoUpdate.json里的IsConsoleReadKey要设置为false,否则该升级程序会一直在后台,不会自动关闭
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await StartAsync(false);
        }
    }
}
