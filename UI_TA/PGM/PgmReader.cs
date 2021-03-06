﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI_TA.PGM
{
    public class PgmReader
    {
        string file;
        Boolean can;
        PgmImage pgmImage;
        int magnify;
        Bitmap bitMap;

        public PgmReader(string file){
            this.file = file;
            can = LoadImage(file, out pgmImage);
            magnify = 1;
            //MessageBox.Show(can.ToString());
            if (can)
            {
                bitMap = MakeBitmap(pgmImage, magnify);
            }
        }

        public Bitmap GetBitmap()
        {
            return this.bitMap;
        }

        public Boolean LoadImage(string file, out PgmImage image)
        {
            FileStream ifs = new FileStream(file, FileMode.Open);
            BinaryReader br = new BinaryReader(ifs);

            string magic = NextNonCommentLine(br);
            if (magic != "P5")
            {
                //throw new Exception("Unknown magic number: " + magic);
                SystemSounds.Beep.Play();
                MessageBox.Show("Unknown magic number: " + magic + "\nFile must be pgm with P5 as magic number");
                image = null;
                return false;
            }
                
            //listBox1.Items.Add("");
            //listBox1.Items.Add("magicer = " + magic);

            string widthHeight = NextNonCommentLine(br);
            string[] tokens = widthHeight.Split(' ');
            int width = int.Parse(tokens[0]);
            int height;
            if (tokens.Length == 2)
                height = int.Parse(tokens[1]);
            else
            {
                widthHeight = NextNonCommentLine(br);
                height = int.Parse(widthHeight);
            }
            //listBox1.Items.Add("widthht = " + width + " " + height);

            if (width * height > 2000000)
            {
                SystemSounds.Beep.Play();
                MessageBox.Show("File to large: " + width * height + "\nFile must under 2,000,000 pixels");
                image = null;
                return false;
            }

            string sMaxVal = NextNonCommentLine(br);
            int maxVal = int.Parse(sMaxVal);
            //listBox1.Items.Add("maxVal+ maxVal);

            // read width * height pixel values . . .
            byte[][] pixels = new byte[height][];
            for (int i = 0; i < height; ++i)
                pixels[i] = new byte[width];

            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    try
                    {
                        pixels[i][j] = br.ReadByte();
                    }
                    catch (Exception e)
                    {
                        pixels[i][j] = (pixels[i - 1][j] <= pixels[i][j - 1] ? pixels[i - 1][j] : pixels[i][j - 1]);
                        Console.WriteLine("Error at i:={0}, j:={1}, e:={2}", i, j, e.ToString());
                    }

            br.Close(); ifs.Close();

            PgmImage result = new PgmImage(width, height, maxVal, pixels);
            //listBox1.Items.Add("imageed");

            image = result;
            return true;
        }

        static string NextAnyLine(BinaryReader br)
        {
            string s = "";
            byte b = 0; // dummy
            while (b != 10) // newline
            {
                b = br.ReadByte();
                char c = (char)b;
                s += c;
            }
            return s.Trim();
        }

        static string NextNonCommentLine(BinaryReader br)
        {
            string s = NextAnyLine(br);
            while (s.StartsWith("#") || s == "")
                s = NextAnyLine(br);
            return s;
        }

        static Bitmap MakeBitmap(PgmImage pgmImage, int mag)
        {
            int width = pgmImage.width * mag;
            int height = pgmImage.height * mag;
            Bitmap result = new Bitmap(width, height);
            Graphics gr = Graphics.FromImage(result);
            for (int i = 0; i < pgmImage.height; ++i)
            {
                for (int j = 0; j < pgmImage.width; ++j)
                {
                    int pixelColor = pgmImage.pixels[i][j];
                    Color c = Color.FromArgb(pixelColor, pixelColor, pixelColor);
                    SolidBrush sb = new SolidBrush(c);
                    gr.FillRectangle(sb, j * mag, i * mag, mag, mag);
                }
            }
            return result;
        }
    }
}
