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
            string File = @"D:\Downloads\2.png";
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
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x)");
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

            Dictionary<string, string> ImgTypeLongNames;
            readonly FramesInfo framesInfo = new FramesInfo();

            public ImagingBitmapInfo(FileStream stream)
            {
                FillLongNameTable();

                var coder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                BitmapSource source = coder.Frames[0];
                var metadata = (BitmapMetadata)source.Metadata;
                try
                {
                    ImageType = ImgTypeLongNames[metadata.Format.ToUpperInvariant()];
                    Metadata = new MetadataInfo(metadata);
                }
                catch (NotSupportedException)
                {
                    Metadata = null;
                    IsMetadataSupported = false;
                }

                Frames = coder.Frames.Count();
                if (Frames > 0) FrameSourceAddRange(coder.Frames.ToArray());

                PixelFormat = source.Format;
                HasPalette = ((source.Palette != null) && (source.Palette.Colors.Count > 0)) ? true : false;
                Palette = source.Palette;
                ImageSize = new Size((float)source.Height, (float)source.Width);
                Dpi = new Size((float)source.DpiX, (float)source.DpiY);
                PixelSize = new Size(source.PixelHeight, source.PixelWidth);
                Masks = source.Format.Masks.ToList();
                BitsPerPixel = source.Format.BitsPerPixel;
                AnimationSupported = coder.CodecInfo.SupportsAnimation;
                Animated = (AnimationSupported && (Frames > 1)) ? true : false;
                Animated = source.HasAnimatedProperties;
                HasThumbnail = coder.Thumbnail != null;
                if (HasThumbnail)
                {
                    Thumbnail = (BitmapImage)coder.Thumbnail.CloneCurrentValue();
                }
            }

            public bool Animated { get; }
            public bool AnimationSupported { get; }
            public int BitsPerPixel { get; }
            public Size Dpi { get; }
            public int Frames { get; }
            public FramesInfo FramesContent => framesInfo;
            public bool HasPalette { get; }
            public bool HasThumbnail { get; }
            public Size ImageSize { get; }
            public string ImageType { get; }
            public bool IsMetadataSupported { get; } = true;
            public List<PixelFormatChannelMask> Masks { get; }
            public MetadataInfo Metadata { get; }
            public BitmapPalette Palette { get; }
            public PixelFormat PixelFormat { get; }
            public Size PixelSize { get; }
            public BitmapImage Thumbnail { get; }

            public enum DeepScanOptions : int
            {
                Default = 0,
                Skip,
                Force
            }

            public enum GrayScaleInfo : int
            {
                None = 0,
                Partial,
                GrayScale,
                Undefined
            }

            private void FillLongNameTable()
            {
                ImgTypeLongNames = new Dictionary<string, string>
            {
                { "GIF", "Unisis Graphic Interchange Format (GIF)" },
                { "JPG", "Joint Photographics Experts Group JFIF/JPEG" },
                { "PNG", "Portable Network Graphics (PNG)" },
                { "TIFF", "Tagged Image File Format (TIFF)" },
                { "BMP", "Windows Bitmap Format (BMP)" },
                { "WMP", "Windows Media Photo File (WMP)" },
                { "ICO", "Windows Icon (ICO)" }
            };
            }

            public string GetFormatLongName(string format) => ImgTypeLongNames[format];

            public class MetadataInfo
            {
                public MetadataInfo(BitmapMetadata metadata)
                {
                    ApplicationName = metadata.ApplicationName;
                    Author = metadata.Author?.ToList();
                    Copyright = metadata.Copyright;
                    CameraManufacturer = metadata.CameraManufacturer;
                    CameraModel = metadata.CameraModel;
                    CameraModel = metadata.Comment;
                    DateTaken = metadata.DateTaken;
                    Format = metadata.Format;
                    Rating = metadata.Rating;
                    Subject = metadata.Subject;
                    Title = metadata.Title;
                }

                public string ApplicationName { get; }
                public List<string> Author { get; }
                public string Copyright { get; }
                public string CameraManufacturer { get; }
                public string CameraModel { get; }
                public string Comment { get; }
                public string DateTaken { get; }
                public string Format { get; }
                public int Rating { get; }
                public string Subject { get; }
                public string Title { get; }
            }

            public System.Drawing.Bitmap ThumbnailToBitmap()
            {
                if (Thumbnail == null) return null;

                var bitmap = new System.Drawing.Bitmap(Thumbnail.DecodePixelWidth, Thumbnail.DecodePixelHeight);
                var stream = new MemoryStream();
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(Thumbnail));
                encoder.Save(stream);
                return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(stream).Clone();
            }

            public void FrameSourceAddRange(BitmapFrame[] bitmapFrames)
            {
                if (bitmapFrames == null) return;

                framesInfo.Frames.AddRange(bitmapFrames.Select(bf => new FramesInfo.Frame()
                {
                    Palette = bf.Palette,
                    FrameSize = new Size(bf.PixelWidth, bf.PixelHeight),
                    FrameDpi = new Size(bf.DpiX, bf.DpiY),
                    PixelFormat = bf.Format,
                    IsGrayScaleFrame = CheckIfGrayScale(bf.Format, bf.Palette, DeepScanOptions.Force),
                    IsBlackWhiteFrame = bf.Format == PixelFormats.BlackWhite
                }));

                framesInfo.Frames.Where(f => (!f.IsGrayScaleFrame & !f.IsBlackWhiteFrame)).All(f => f.IsColorFrame);
            }

            public GrayScaleInfo IsGrayScaleFrames()
            {
                if (framesInfo.Frames.Count == 0) return GrayScaleInfo.Undefined;
                if (framesInfo.FramesGrayscaleNumber > 0)
                {
                    return (framesInfo.FramesGrayscaleNumber == framesInfo.FramesTotalNumber)
                            ? GrayScaleInfo.GrayScale
                            : GrayScaleInfo.Partial;
                }
                return GrayScaleInfo.None;
            }

            public bool IsGrayScale(DeepScanOptions DeepScan) => CheckIfGrayScale(PixelFormat, Palette, DeepScan);

            private bool CheckIfGrayScale(PixelFormat pixelFormat, BitmapPalette palette, DeepScanOptions DeepScan)
            {
                if (pixelFormat == PixelFormats.Gray32Float ||
                    pixelFormat == PixelFormats.Gray16 ||
                    pixelFormat == PixelFormats.Gray8 ||
                    pixelFormat == PixelFormats.Gray4 ||
                    pixelFormat == PixelFormats.Gray2)
                {
                    if (palette == null || (DeepScan != DeepScanOptions.Force)) { return true; }
                }

                if (pixelFormat == PixelFormats.Indexed8 ||
                    pixelFormat == PixelFormats.Indexed4 ||
                    pixelFormat == PixelFormats.Indexed2)
                {
                    DeepScan = (DeepScan != DeepScanOptions.Skip) ? DeepScanOptions.Force : DeepScan;
                }

                if ((DeepScan != DeepScanOptions.Skip) & palette != null)
                {
                    List<Color> indexedColors = palette.Colors.ToList();
                    return indexedColors.All(rgb => (rgb.R == rgb.G && rgb.G == rgb.B && rgb.B == rgb.R));
                }
                return false;
            }

            public GrayScaleStats GrayScaleSimilarity()
            {
                if (!HasPalette) return null;
                return new GrayScaleStats(framesInfo.Frames);
            }

            public class GrayScaleStats
            {
                public GrayScaleStats(IEnumerable<FramesInfo.Frame> frames)
                {

                    float accumulatorMax = 0F;
                    float accumulatorMin = 0F;
                    float accumulatorAvg = 0F;
                    float[] distance = new float[3];

                    Palettes = frames.Count();

                    foreach (FramesInfo.Frame frame in frames)
                    {

                        foreach (Color pEntry in frame.Palette.Colors)
                        {
                            if (!(pEntry.R == pEntry.G && pEntry.G == pEntry.B && pEntry.B == pEntry.R))
                            {
                                distance[0] = Math.Abs(pEntry.R - pEntry.G);
                                distance[1] = Math.Abs(pEntry.G - pEntry.B);
                                distance[2] = Math.Abs(pEntry.B - pEntry.R);
                                accumulatorMax += distance.Max();
                                accumulatorMin += distance.Min();
                                accumulatorAvg += distance.Average();
                            }
                        }
                        int colorEntries = frame.Palette.Colors.Count;

                        var distanceAverage = (float)((accumulatorAvg / 2.56) / colorEntries);
                        var distanceMax = (float)((accumulatorMax / 2.56) / colorEntries);
                        var distanceMin = (float)((accumulatorMin / 2.56) / colorEntries);

                        var framestat = new FrameStat(colorEntries, distanceAverage, distanceMax, distanceMin);
                        PerFrameValues.Add(framestat);
                        accumulatorAvg = 0F;
                        accumulatorMax = 0F;
                        accumulatorMin = 0F;
                    }

                    AverageLogDistance = PerFrameValues.Average(avg => avg.DistanceAverage);
                    AverageMaxDistance = PerFrameValues.Max(mx => mx.DistanceMax);
                    AverageMinDistance = PerFrameValues.Min(mn => mn.DistanceMin);
                    GrayScaleAveragePercent = 100F - AverageLogDistance;
                    GrayScalePercent = 100F - ((AverageMaxDistance - AverageMinDistance) / 2);
                }

                public float AverageMaxDistance { get; }
                public float AverageMinDistance { get; }
                public float AverageLogDistance { get; }
                public float GrayScalePercent { get; }
                public float GrayScaleAveragePercent { get; }
                public int Palettes { get; }
                public List<FrameStat> PerFrameValues { get; } = new List<FrameStat>();

                public class FrameStat
                {
                    public FrameStat(int entries, float average, float max, float min)
                    {
                        ColorEntries = entries;
                        DistanceAverage = average;
                        DistanceMax = max;
                        DistanceMin = min;
                    }
                    public int ColorEntries { get; private set; }
                    public float DistanceAverage { get; private set; }
                    public float DistanceMax { get; private set; }
                    public float DistanceMin { get; private set; }
                }
            }

            public class FramesInfo
            {
                public FramesInfo() => Frames = new List<Frame>();

                public int FramesTotalNumber
                {
                    get => (Frames != null) ? Frames.Count() : 0;
                    private set { }
                }

                public int FramesColorNumber
                {
                    get => (Frames != null) ? Frames.Where(f => f.IsColorFrame == true).Count() : 0;
                    private set { }
                }
                public int FramesGrayscaleNumber
                {
                    get => (Frames != null) ? Frames.Where(f => f.IsGrayScaleFrame == true).Count() : 0;
                    private set { }
                }

                public int FramesBlackWhiteNumber
                {
                    get => (Frames != null) ? Frames.Where(f => f.IsBlackWhiteFrame == true).Count() : 0;
                    private set { }
                }

                public List<Frame> Frames { get; private set; }

                public class Frame
                {
                    public BitmapPalette Palette { get; set; }
                    public Size FrameSize { get; set; }
                    public Size FrameDpi { get; set; }
                    public PixelFormat PixelFormat { get; set; }
                    public bool IsColorFrame { get; set; }
                    public bool IsGrayScaleFrame { get; set; }
                    public bool IsBlackWhiteFrame { get; set; }
                }
            }
        }
    }
}
