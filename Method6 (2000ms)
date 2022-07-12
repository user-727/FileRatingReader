using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Tests
{
    public partial class Form1 : Form
    {
        List<string> Files = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void event_Form1_Shown(object sender, EventArgs e)
        {
            string File = @"D:\Downloads\1.jpg";
            int NumberOfLoops = 5000;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < NumberOfLoops; i++)
            {
                FileStream streamI = new FileStream(File, FileMode.Open);
                JpegBitmapDecoder ImageDecoder = new JpegBitmapDecoder(streamI, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                BitmapMetadata metadata = (BitmapMetadata)ImageDecoder.Frames[0].Metadata;

                if (metadata.Rating == 0)
                {
                    Files.Add(File);
                }

                streamI.Close();
                streamI.Dispose();
            }

            sw.Stop();
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x) " + Files.Count.ToString());
        }
    }
}
