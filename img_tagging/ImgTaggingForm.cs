using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.IO;

namespace img_tagging
{
    public partial class ImgTaggingForm : Form
    {
        public ImgTaggingForm()
        {
            InitializeComponent();
        }

        // Archive directory name.
        private const string ARCHIVE_DIR = "archive";
        // Tags library file, stores tags and their members.
        private const string TAGS_LIBRARY_FILE = "tags.json";

        private void btnTag_Click(object sender, EventArgs e)
        {
            //string[] tags = new string[3] { "Good", "Bad", "Nah" };
            //// read file.
            //var shellobj = Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName("C:\\Users\\ntin\\Pictures\\nishi\\tumblr_adventure_1280.jpg");
            //// print Tags property to file.
            //foreach (string tag in shellobj.Properties.System.Keywords.Value)
            //{
            //    Console.WriteLine(tag);
            //}
            //// get writer of the property.
            //ShellPropertyWriter w = shellobj.Properties.GetPropertyWriter();
            //// write new value of the property.
            //w.WriteProperty(SystemProperties.System.Keywords, tags);
            //// close the writer.
            //w.Close();

            tag(txtPath.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = pathBrowseDialog.ShowDialog();
            
            if(result == DialogResult.OK 
                && !String.IsNullOrWhiteSpace(pathBrowseDialog.SelectedPath))
            {
                txtPath.Text = pathBrowseDialog.SelectedPath;
            }
        }

        private void pathBrowseDialog_HelpRequest(object sender, EventArgs e)
        {

        }

        private void tag(string rootPath)
        {
            // List all directory found in the rootPath except for ARCHIVE_DIR.
            var dirs = Directory.GetDirectories(rootPath).Where(d => Path.GetFileName(d) != ARCHIVE_DIR);
            // Clear the task logs.
            txtTaskLogs.Clear();
            // Clear the progress bar.
            tagProgress.Value = 0;

            if(dirs.ToArray().Length == 0)
            {
                // No directory is found!
                log("Not found directory.");
                return; // End this process.
            }

            int task_count = dirs.ToArray().Length;
            int task_done = 0;

            foreach(string d in dirs)
            {
                tagdir(d);

                task_done++; // Increase task_done count.
                updateTaskProgressBar(task_done, task_count);
            }
        }

        private void log(string msg)
        {
            if (txtTaskLogs.Text.Length != 0)
            {
                txtTaskLogs.AppendText("\r\n");
            }
            txtTaskLogs.AppendText(msg);
        }

        private void updateTaskProgressBar(int done, int all)
        {
            tagProgress.Value = (done / all) * 100;
        }

        private void tagdir(string path)
        {
            log("Tagging images in directory: " + path);

        }
    }
}
