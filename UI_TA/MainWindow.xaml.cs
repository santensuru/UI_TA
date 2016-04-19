using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace UI_TA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process compiler;
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            compiler = new Process();
            compiler.StartInfo.FileName = "main.exe";
            compiler.StartInfo.Arguments = "";
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.CreateNoWindow = true;
            compiler.StartInfo.ErrorDialog = false;
            compiler.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            compiler.Start();

            //Console.WriteLine(compiler.StandardOutput.ReadToEnd());

            text.Text = compiler.StandardOutput.ReadToEnd();

            compiler.WaitForExit();
        }
    }
}
