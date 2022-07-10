using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using System.Diagnostics;
using Shell32;

namespace Tests
{
    public partial class Form1 : Form
    {
        List<string> Files = new List<string>();
        Shell32.Shell app = new Shell32.Shell();

        public Form1()
        {
            InitializeComponent();
        }

        private void event_Form1_Shown(object sender, EventArgs e)
        {
            int NumberOfLoops = 5000;
            string FileName = "1.png";
            string Folder = @"D:\Downloads\";
            Folder folder = app.NameSpace(Folder);
            FolderItems items = folder.Items();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < NumberOfLoops; i++)
            {
                foreach (FolderItem2 item in items) 
                {
                    var itemRating = (int?)item.ExtendedProperty("System.Rating");
                    var itemName = (string)item.ExtendedProperty("Name");

                    if (itemRating == null && itemName == FileName)
                    {
                        Files.Add(Folder + FileName);
                    }
                }
            }

            sw.Stop();
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x)");
        }
    }
}
