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
using Newtonsoft.Json;
using System.Text.RegularExpressions;

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
        // Rexeg pattern to detect image file.
        // JPG, PNG files.
        private const string IMG_FILE_PATTERN = "(.*)(\\.(jpg|png))$";
        // Instance var for tags library file.
        private Tags taglib_ = null;

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
            // check selected path first.
            if(string.IsNullOrEmpty(rootPath))
            {
                MessageBox.Show("Please select a folder first.");
                return;
            }

            // List all directory found in the rootPath except for ARCHIVE_DIR.
            var dirs = Directory.GetDirectories(rootPath).Where(d => Path.GetFileName(d) != ARCHIVE_DIR);
            start();

            if(dirs.ToArray().Length == 0)
            {
                // No directory is found!
                log("Not found directory.");
            } else
            {
                log("Found some directory. Process to tagging process...");

                loadTagLib(rootPath);

                int task_count = dirs.ToArray().Length;
                int task_done = 0;

                foreach (string d in dirs)
                {
                    tagdir(d);

                    task_done++; // Increase task_done count.
                    updateTaskProgressBar(task_done, task_count);
                }
            }

            end();
        }

        private void start()
        {
            // Clear the task logs.
            txtTaskLogs.Clear();
            // Clear the progress bar.
            tagProgress.Value = 0;
            // Disable all buttons.
            btnBrowse.Enabled = false;
            btnTag.Enabled = false;
        }

        private void end()
        {
            // Enable buttons.
            btnBrowse.Enabled = true;
            btnTag.Enabled = true;
        }

        private void loadTagLib(string rootPath)
        {
            log("Look for a tag library file (tags.json).");

            string file = rootPath + "\\" + TAGS_LIBRARY_FILE;
            if (File.Exists(file))
            {
                log("Found it, try to load to list.");

                using (StreamReader reader = new StreamReader(file))
                {
                    string json = reader.ReadToEnd();
                    taglib_ = JsonConvert.DeserializeObject<Tags>(json);

                    log("Loading success.");

                    if(taglib_.Count() == 0)
                    {
                        log("It seems tag library file is empty.");
                    }
                }
                
            } else
            {
                log("Not found any tag library file.");
                taglib_ = new Tags();
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

        private bool IsAnImageFile(string filename)
        {
            return Regex.Match(filename, IMG_FILE_PATTERN).Success;
        }

        private void tagdir(string path)
        {
            log("Tagging images in directory: " + path);

            // 1. List all image files.
            var imgs = Directory.GetFiles(path).Where(f => IsAnImageFile(Path.GetFileName(f)));

            if(imgs.ToArray().Length == 0)
            {
                log("Not found any image file.");
            } else
            {
                foreach (string i in imgs)
                {
                    string fname = Path.GetFileName(i);
                    log("Image file: " + fname);


                }
            }

            
        }
    }
}
