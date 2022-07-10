using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

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
            string Folder = @"D:\Downloads\";
            string File = "1.png";
            int NumberOfLoops = 5000;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < NumberOfLoops; i++)
            {
                var folderObj = app.NameSpace(Folder);
                var filesObj = folderObj.Items();

                var headers = new Dictionary<string, int>();
                for (int j = 0; j < short.MaxValue; j++)
                {
                    string header = folderObj.GetDetailsOf(null, j);
                    if (String.IsNullOrEmpty(header))
                        break;
                    if (!headers.ContainsKey(header)) headers.Add(header, j);
                }

                var testFile = filesObj.Item(File);

                if (folderObj.GetDetailsOf(testFile, headers["Rating"]) == "Unrated")
                {
                    Files.Add(Folder + File);
                }
            }

            sw.Stop();
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x)");
        }
    }
}
