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
        Boolean lzw = false;
        Boolean transition = false;
        Boolean fl = false;
        int _fl = 1;

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
            openFileDialog.Filter = "Netpbm Files (*.pgm)|*.pgm";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                input.Text = openFileDialog.FileName;

                execute.IsEnabled = true;

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

                if (this.lzw)
                {
                    tr.IsEnabled = true;
                    fls.IsEnabled = true;
                }
            }
            else if (radioButton.Content.ToString().Equals("Mode"))
            {
                this.metode = "Mode";
                this.argument = "ls 1";

                if (this.lzw)
                {
                    tr.IsEnabled = true;
                    fls.IsEnabled = true;
                }
            }
            else if (radioButton.Content.ToString().Equals("Mean"))
            {
                this.metode = "Mean";
                this.argument = "ls 2";

                if (this.lzw)
                {
                    tr.IsEnabled = true;
                    fls.IsEnabled = true;
                }
            }
            else if (radioButton.Content.ToString().Equals("Adaptive"))
            {
                this.metode = "Adaptive";
                this.argument = "la";

                fls.IsEnabled = false;
                fls.IsChecked = false;
                this.fl = false;
                this._fl = 1;
                dd.IsEnabled = false;
                dd.SelectedIndex = 0;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "UI - Grayscale Image Compression base-on Adaptive Huffman and LZW Algorithm";
            
            this.Left = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - this.Width) / 2;
            this.Top = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("1");
            data.Add("2");
            data.Add("3");
            data.Add("4");
            data.Add("5");
            data.Add("6");
            data.Add("7");
            data.Add("8");

            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            this._fl = Int32.Parse(value);
        }

        public void Load(string filename, System.Windows.Controls.Image output, TextBlock outputName)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    PGM.PgmReader reader = new PGM.PgmReader(filename);
                    Bitmap bitmap = reader.GetBitmap();
                    if (bitmap != null)
                    {
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
                    else
                    {
                        execute.IsEnabled = false;
                    }
                    
                }
            })); 
        }

        public void Execute() {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string curFile;
                string curArg = "";

                text.Text = "Metode Terpilih: " + metode + (this.lzw ? " + LZW" : "") + (this.transition ? " alur transisi" : " 8 bit") + (this.fl ? " + fix_length:=" + this._fl : "") + "\n";

                // encode
                compiler = new Process();
                compiler.StartInfo.FileName = "main.exe";
                if (this.lzw)
                {
                    curArg += "Lt";
                }
                if (this.transition)
                {
                    curArg += "b";
                }
                curArg += this.argument;
                if (this.fl)
                {
                    curArg += " -f " + this._fl.ToString() + "..." + this._fl.ToString();
                }
                //MessageBox.Show(curArg);
                compiler.StartInfo.Arguments = "-c" + curArg + " -i \"" + input.Text + "\"";
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
                if (this.lzw && !this.fl)
                {
                    curFile = input.Text.Replace(".pgm", ".ahlzwo");
                }
                else if (this.lzw && this.fl)
                {
                    curFile = input.Text.Replace(".pgm", ".fl-" + this._fl.ToString() + ".ahlzwo");
                }
                else
                {
                    curFile = input.Text.Replace(".pgm", ".ah");
                }
                this.filenames.Add(curFile);
                compiler.StartInfo.Arguments = "-d" + curArg + " -i \"" + curFile + "\"";
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
                curFile = curFile.Replace(".ah", ".restore");
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

                Thread thread2 = new Thread(() => Load(curFile, akhir, akhirName));
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

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (this.lzw == false)
            {
                this.lzw = true;
                tr.IsEnabled = true;
                
                if (!this.metode.Equals("Adaptive"))
                {
                    fls.IsEnabled = true;
                }
            }
            else
            {
                this.lzw = false;
                this.transition = false;
                tr.IsEnabled = false;
                tr.IsChecked = false;
                fls.IsEnabled = false;
                this.fl = false;
                this._fl = 1;
                fls.IsChecked = false;
                dd.IsEnabled = false;
                dd.SelectedIndex = 0;
            }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.fl == false)
            {
                this.fl = true;
                dd.IsEnabled = true;
            }
            else
            {
                this.fl = false;
                this._fl = 1;
                dd.IsEnabled = false;
                dd.SelectedIndex = 0;
            }

            //MessageBox.Show(this.fl.ToString());
        }

        private void CheckBox_Click_2(object sender, RoutedEventArgs e)
        {
            if (this.lzw == true)
            {
                if (this.transition == false)
                {
                    this.transition = true;
                }
                else
                {
                    this.transition = false;
                }
            }
        }
    }
}
