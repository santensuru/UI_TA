using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
        List<string> filenames;

        Process compiler;
        string argument = "ls 0";
        string metode = "Median";

        public MainWindow()
        {
            InitializeComponent();
            filenames = new List<string>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (input.Text.Equals(""))
            {
                SystemSounds.Beep.Play();
                MessageBox.Show("Harap Masukkan File Path");
            }
            else
            {
                Thread thread = new Thread(() => Execute());
                thread.Start();
            }
        }

        private void Open_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                input.Text = openFileDialog.FileName;

                Thread thread = new Thread(() => Load(openFileDialog.FileName, awal, awalName));
                thread.Start();
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;

            if (radioButton.Content.ToString().Equals(this.metode))
            {
                SystemSounds.Beep.Play();
                MessageBox.Show("Metode yang dipilih tetap");
            }
            else if (radioButton.Content.ToString().Equals("Median"))
            {
                this.metode = "Median";
                this.argument = "ls 0";
            }
            else if (radioButton.Content.ToString().Equals("Mode"))
            {
                this.metode = "Mode";
                this.argument = "ls 1";
            }
            else if (radioButton.Content.ToString().Equals("Mean"))
            {
                this.metode = "Mean";
                this.argument = "ls 2";
            }
            else if (radioButton.Content.ToString().Equals("Adaptive"))
            {
                this.metode = "Adaptive";
                this.argument = "la";
            }
            else if (radioButton.Content.ToString().Equals("LZW + Mean"))
            {
                this.metode = "LZW + Mean";
                this.argument = "lLts 2";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - this.Width) / 2;
            this.Top = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
        }

        public void Load(string filename, System.Windows.Controls.Image output, TextBlock outputName)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    PGM.PgmReader reader = new PGM.PgmReader(filename);
                    Bitmap bitmap = reader.GetBitmap();
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    output.Source = bitmapImage;
                    output.Stretch = Stretch.Uniform;

                    if (filename.Contains(".restore"))
                    {
                        string name = filename;
                        name = filename.Replace(".restore", ".ah");
                        outputName.Text = "Compressed Size: " + new System.IO.FileInfo(name).Length.ToString() + " bytes\n";
                        outputName.Text += "Restored Size: " + new System.IO.FileInfo(filename).Length.ToString() + " bytes";
                    }
                    else
                    {
                        outputName.Text = "Original Size: " + new System.IO.FileInfo(filename).Length.ToString() + " bytes";
                    }
                    
                }
            })); 
        }

        public void Execute() {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string curFile;
                
                text.Text = "Metode Terpilih: " + metode + "\n";

                // encode
                compiler = new Process();
                compiler.StartInfo.FileName = "main.exe";
                compiler.StartInfo.Arguments = "-c" + argument + " -i \"" + input.Text + "\"";
                compiler.StartInfo.UseShellExecute = false;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.CreateNoWindow = true;
                compiler.StartInfo.ErrorDialog = false;
                compiler.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                compiler.Start();

                text.Text += compiler.StandardOutput.ReadToEnd();

                compiler.WaitForExit();

                // decode
                compiler = new Process();
                compiler.StartInfo.FileName = "main.exe";
                if (argument.Equals("lLts 2"))
                {
                    curFile = input.Text.Replace(".pgm", ".ahlzwo");
                }
                else
                {
                    curFile = input.Text.Replace(".pgm", ".ah");
                }
                this.filenames.Add(curFile);
                compiler.StartInfo.Arguments = "-d" + argument + " -i \"" + curFile + "\"";
                compiler.StartInfo.UseShellExecute = false;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.CreateNoWindow = true;
                compiler.StartInfo.ErrorDialog = false;
                compiler.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                compiler.Start();

                text.Text += compiler.StandardOutput.ReadToEnd();

                compiler.WaitForExit();

                // psnr
                compiler = new Process();
                compiler.StartInfo.FileName = "psnr.exe";
                if (argument.Equals("lLts 2"))
                {
                    curFile = input.Text.Replace(".pgm", ".restorelzwo");
                }
                else
                {
                    curFile = input.Text.Replace(".pgm", ".restore");
                }
                this.filenames.Add(curFile);
                compiler.StartInfo.Arguments = "\"" + input.Text + "\" \"" + curFile + "\"";
                compiler.StartInfo.UseShellExecute = false;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.CreateNoWindow = true;
                compiler.StartInfo.ErrorDialog = false;
                compiler.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                compiler.Start();

                text.Text += "\n" + compiler.StandardOutput.ReadToEnd();

                compiler.WaitForExit();

                string filename;
                if (argument.Equals("lLts 2"))
                {
                    filename = input.Text.Replace(".pgm", ".restorelzwo");
                }
                else
                {
                    filename = input.Text.Replace(".pgm", ".restore");
                }

                Thread thread2 = new Thread(() => Load(filename, akhir, akhirName));
                thread2.Start();
            }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (string filename in filenames)
            {
                File.Delete(filename);
            }
        }
    }
}
