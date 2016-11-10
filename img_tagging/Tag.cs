using System.Collections.Generic;
using System.Text;

namespace img_tagging.tag
{
    class Tag
    {
        public Tag()
        {
            // default.
        }

        public Tag(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public HashSet<string> Members { get; set; }

        public void AddMembers(params string[] members)
        {
            if(Members == null)
            {
                Members = new HashSet<string>();
            }
            foreach(string m in members)
            {
                Members.Add(m);
            }
        }

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
                    .Append(string.Join(", ", Members))
                    .AppendLine(" ]");
            }
            return s.ToString();
        }
    }
}
