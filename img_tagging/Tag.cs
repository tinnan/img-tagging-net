using System.Collections.Generic;
using System.Text;

namespace img_tagging
{
    class Tag
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Members { get; set; }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("Tag name: ")
                .AppendLine(Name)
                .Append("Description: ")
                .AppendLine(Description);
            if(Members != null)
            {
                s.Append("Members: [ ")
                    .Append(string.Join(", ", Members.ToArray()))
                    .AppendLine(" ]");
            }
            return s.ToString();
        }
    }
}
