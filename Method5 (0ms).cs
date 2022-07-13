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
            var bitmapInfo = ImagingBitmapInfo.BitmapPixelFormat(File);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < NumberOfLoops; i++)
            {
                int Rating = bitmapInfo.Metadata?.Rating ?? 0;

                if (Rating == 0)
                {
                    Files.Add(File);
                }
            }

            sw.Stop();
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x) " + Files.Count.ToString());
        }

        public class ImagingBitmapInfo
        {
            public static ImagingBitmapInfo BitmapPixelFormat(string FileName)
            {
                using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return BitmapPixelFormat(stream);
                }
            }

            public static ImagingBitmapInfo BitmapPixelFormat(FileStream stream) => new ImagingBitmapInfo(stream);

            public ImagingBitmapInfo(FileStream stream)
            {
                var coder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                BitmapSource source = coder.Frames[0];
                var metadata = (BitmapMetadata)source.Metadata;
                try
                {
                    Metadata = new MetadataInfo(metadata);
                }
                catch (NotSupportedException)
                {
                    Metadata = null;
                    IsMetadataSupported = false;
                }
            }

            public bool IsMetadataSupported { get; } = true;
            public MetadataInfo Metadata { get; }

            public class MetadataInfo
            {
                public MetadataInfo(BitmapMetadata metadata)
                {
                    Rating = metadata.Rating;
                }

                public int Rating { get; }
            }
        }
    }
}
