using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

        private void Open_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                input.Text = openFileDialog.FileName;

                using (MemoryStream memory = new MemoryStream())
                {
                    PGM.PgmReader reader = new PGM.PgmReader(openFileDialog.FileName);
                    Bitmap bitmap = reader.GetBitmap();
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    awal.Source = bitmapImage;
                    awal.Stretch = Stretch.Uniform;
                }
                //BitmapImage src = new BitmapImage();
                //src.BeginInit();
                //src.UriSource = new Uri(openFileDialog.FileName, UriKind.Relative);
                //src.CacheOption = BitmapCacheOption.OnLoad;
                //src.EndInit();
                //awal.Source = src;
                //awal.Stretch = Stretch.Uniform;
            }
                


        }

        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }
    }
}
