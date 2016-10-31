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

namespace img_tagging
{
    public partial class ImgTaggingForm : Form
    {
        public ImgTaggingForm()
        {
            InitializeComponent();
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            string[] tags = new string[3] { "Good", "Bad", "Nah" };
            // read file.
            var shellobj = Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName("C:\\Users\\ntin\\Pictures\\nishi\\tumblr_adventure_1280.jpg");
            // print Tags property to file.
            foreach (string tag in shellobj.Properties.System.Keywords.Value) {
                Console.WriteLine(tag);
            }
            // get writer of the property.
            ShellPropertyWriter w = shellobj.Properties.GetPropertyWriter();
            // write new value of the property.
            w.WriteProperty(SystemProperties.System.Keywords, tags);
            // close the writer.
            w.Close();
        }
    }
}
