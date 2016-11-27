using System.Collections.Generic;
using System.Text;

namespace img_tagging.tag
{
    public class Tag
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
        public string Type {
            get
            {
                return ToTagTypeString(type_);
            }
            set
            {
                type_ = ToTagTypeEnum(value);
            }
        }
        public string Description { get; set; }
        public HashSet<string> Members { get; set; } = new HashSet<string>();

        private TagType type_ = TagType.T; // Default as normal tag.

        public void AddMembers(params string[] members)
        {
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
                .Append("Type: ")
                .AppendLine(Type)
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

        static public string ToTagTypeString(TagType type)
        {
            return type.ToString();
        }

        static public TagType ToTagTypeEnum(string type)
        {
            if (TagType.S.ToString().Equals(type, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return TagType.S;
            } else if (TagType.A.ToString().Equals(type, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return TagType.A;
            }

            return TagType.T; // Normal tag type as default.
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tag))
            {
                return false;
            }

            Tag that = (Tag)obj;

            // Compare Tag name.
            string thisName = Name;
            string thatName = that.Name;
            if (!string.Equals(thisName, thatName))
            {
                return false;
            }

            // Compare Tag type.
            string thisType = Type;
            string thatType = that.Type;
            if (!string.Equals(thisType, thatType))
            {
                return false;
            }

            // Compare Tag description.
            string thisDesc = Description;
            string thatDesc = that.Description;
            if (!string.Equals(thisDesc, thatDesc))
            {
                return false;
            }

            // Compare Members.
            ISet<string> thisMembers = Members;
            ISet<string> thatMembers = that.Members;
            if (thisMembers.Count != thatMembers.Count)
            {
                return false;
            }

            foreach (string m in thisMembers)
            {
                if (!thatMembers.Contains(m))
                {
                    return false;
                }
            }

            // At last.
            return true;
        }
    }

    public enum TagType
    {
        /// <summary>
        /// Tag type: Site
        /// </summary>
        S,
        /// <summary>
        /// Tag type: Actress
        /// </summary>
        A, 
        /// <summary>
        /// Tag type: Tag
        /// </summary>
        T
    }
}
