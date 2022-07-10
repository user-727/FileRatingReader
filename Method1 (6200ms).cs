using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Diagnostics;

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
            string File = @"D:\Downloads\1.png";
            int NumberOfLoops = 5000;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < NumberOfLoops; i++)
            {
                var file = ShellFile.FromFilePath(File);
                int Rating = Convert.ToInt32(file.Properties.System.Rating.Value);

                if (Rating == 0)
                {
                    Files.Add(File);
                }
            }

            sw.Stop();
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x)");
        }
    }
}
