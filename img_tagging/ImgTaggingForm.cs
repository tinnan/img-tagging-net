using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using img_tagging.tag;
using img_tagging.tag.converter;
using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.IO.Compression;

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
        // JPG, PNG, GIF files.
        private const string IMG_FILE_PATTERN = "(.*)(\\.(jpg|png|gif))$";
        // Pattern to detect actress name tags.
        // starts and ends with single quote.
        private const string ACT_NAME_PATTERN = "^'.*'$";
        // Progress text template.
        private const string PRG_TEXT_TEMPLATE = "Tagging {0} of {1} image directory(s).";

        private void btnTag_Click(object sender, EventArgs e)
        {
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
            Start();

            if(dirs.ToArray().Length == 0)
            {
                // No directory is found!
                Log("Not found directory.");
            } else
            {
                Log("Found some directory. Proceed to tagging process...");

                Tags taglib = LoadTagLib(rootPath);

                int task_count = dirs.ToArray().Length;
                int task_done = 0;

                foreach (string d in dirs)
                {
                    tagdir(d, taglib);

                    task_done++; // Increase task_done count.
                    UpdateProgressLabel(task_done, task_count);
                }

                // Write to taglib file.
                WriteTagLib(taglib, rootPath);
            }

            End();
        }

        private void Start()
        {
            // Clear the task logs.
            txtTaskLogs.Clear();
            // Clear progress.
            txtProgress.Clear();
            // Disable all buttons.
            btnBrowse.Enabled = false;
            btnTag.Enabled = false;
        }

        private void End()
        {
            // Enable buttons.
            btnBrowse.Enabled = true;
            btnTag.Enabled = true;
        }

        private void WriteTagLib(Tags taglib, string rootPath)
        {
            Log("Writing tags.json file.");

            string json = JsonConvert.SerializeObject(taglib);
            File.WriteAllText(Path.Combine(rootPath, TAGS_LIBRARY_FILE), json);
        }

        private Tags LoadTagLib(string rootPath)
        {
            Log("Look for a tag library file (tags.json).");

            Tags taglib;

            string file = Path.Combine(rootPath, TAGS_LIBRARY_FILE);
            if (File.Exists(file))
            {
                Log("Found it, try to load to list.");

                using (StreamReader reader = new StreamReader(file))
                {
                    string json = reader.ReadToEnd();
                    taglib = JsonConvert.DeserializeObject<Tags>(json, new TagsConverter());

                    Log("Loading success.");
                }
                
            } else
            {
                Log("Not found any tag library file.");
                taglib = new Tags();
            }

            return taglib;
        }

        private void Log(string msg)
        {
            if (txtTaskLogs.Text.Length != 0)
            {
                txtTaskLogs.AppendText("\r\n");
            }
            txtTaskLogs.AppendText(msg);
        }

        private void UpdateProgressLabel(int done, int all)
        {
            string text = String.Format(PRG_TEXT_TEMPLATE, done, all);
            if (all > 0 && done == all)
            {
                text += " Completed!";
            }
            txtProgress.Clear();
            txtProgress.AppendText(text);
        }

        private bool IsAnImageFile(string filename)
        {
            return Regex.Match(filename, IMG_FILE_PATTERN).Success;
        }

        private void tagdir(string path, Tags taglib)
        {
            Log("Tagging images in directory: " + path);

            // Directory name.
            string dir = Path.GetFileName(path);

            // 1. List all image files.
            var imgs = Directory.GetFiles(path).Where(f => IsAnImageFile(Path.GetFileName(f)));

            if(imgs.Count() == 0)
            {
                Log("Not found any image file.");
                return;
            }

            // 2. Derive tags from directory name.
            string[] tags = DeriveTagList(dir);
            // 3. Add site tag to tag lib.
            taglib.AddSite(tags[0]); // first tag is always a Site.
            // 4. Add actress tag to tag lib.
            foreach(string a in tags.ToList<string>()
                .Where(t => { return Regex.Match(t, ACT_NAME_PATTERN).Success; })) // filter only tags that start and end with a songle quote
                                                                           // a.k.a. Actress name.
            {
                string ac = a.Replace("'", ""); // remove leading and trailing single quotes.
                taglib.AddActress(ac);
            }

            string[] refinedtags = RefineTagList(tags); // refine the tags. 

            IList<string> imgnames = new List<string>(imgs.Count());
            // 5. Write extended properties to files.
            foreach(string fpath in imgs)
            {
                WriteTagsToFile(fpath, refinedtags);
                // Collect file name to new list.
                string fname = Path.GetFileName(fpath);
                imgnames.Add(fname);
            }
            
            // 6. Add all image file names as member of every tags.
            foreach(string tagname in refinedtags)
            {
                Tag tag = taglib.GetOrCreateTag(tagname);
                tag.AddMembers(imgnames.ToArray());
            }

            // 7. Copy all files to parent directory.
            CopyImgFilesToParent(path, imgs.ToArray());

            // 8. Archive this image directory.
            ArchiveDirectory(path);
        }

        private string[] DeriveTagList(string dir)
        {
            return dir.Split(' '); // split dir name with space char.
        }

        private string[] RefineTagList(string[] tags)
        {
            for(int i = 0; i < tags.Length; i++)
            {
                tags[i] = tags[i].Replace("'", ""); 
            }
            return tags;
        }

        private void WriteTagsToFile(string fpath, string[] tags)
        {
            Log("Writing tags to file: " + Path.GetFileName(fpath));
            // read file.
            var shellobj = Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(fpath);
            using (ShellPropertyWriter w = shellobj.Properties.GetPropertyWriter()) // get writer of the property.
            {
                // write new value of the property.
                w.WriteProperty(SystemProperties.System.Keywords, tags);
            }
        }

        private void CopyImgFilesToParent(string path, string[] imgs)
        {
            string parentpath = Path.GetDirectoryName(path); // get parent dir path.
            foreach (string i in imgs)
            {
                string fname = Path.GetFileName(i);
                File.Copy(i, Path.Combine(parentpath, fname), true); // copy with overwrite option.
            }
        }

        private void ArchiveDirectory(string path)
        {
            string parentpath = Path.GetDirectoryName(path); // get parent dir path.
            string arch_dir = Path.Combine(parentpath, ARCHIVE_DIR);
            if (!Directory.Exists(arch_dir))
            {
                Directory.CreateDirectory(arch_dir);
            }
            
            string dname = Path.GetFileName(path); // get this image directory name.
            ZipFile.CreateFromDirectory(path, Path.Combine(arch_dir, dname + ".zip")); // create archive file
                                                                                                      // in ARCHIVE_DIR.                                    
            Directory.Delete(path, true); // delete the directory.
        }
    }
}
