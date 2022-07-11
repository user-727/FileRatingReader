using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using Shell32;

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
                string Rating = GetExtendedFileAttribute(File, "Rating");

                if (Convert.ToInt32(Rating) == 0)
                {
                    Files.Add(File);
                }
            }

            sw.Stop();
            MessageBox.Show("Time: " + sw.ElapsedMilliseconds.ToString() + "ms (" + NumberOfLoops.ToString() + "x)");
        }

        public static string GetExtendedFileAttribute(string filePath, string propertyName)
        {
            string retValue = null;
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            object shell = Activator.CreateInstance(shellAppType);
            Shell32.Folder folder = (Shell32.Folder)shellAppType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { @"C:\Windows\System32" });
            int? foundIdx = null;
            for (int i = 0; i < short.MaxValue; i++)
            {
                string header = folder.GetDetailsOf(null, i);
                if (header == propertyName)
                {
                    foundIdx = i;
                    break;
                }
            }

            if (foundIdx.HasValue)
            {
                foreach (FolderItem2 item in folder.Items())
                {
                    if (item.Name.ToUpper() == System.IO.Path.GetFileName(filePath).ToUpper())
                    {
                        retValue = folder.GetDetailsOf(item, foundIdx.GetValueOrDefault());
                        break;
                    }
                }
            }

            return retValue;
        }
    }
}
