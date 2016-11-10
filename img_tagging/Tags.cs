﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace img_tagging.tag
{
    class Tags
    {
        
        public List<string> Sites { set; get; }
        public List<string> Actresses { set; get; }
        public List<Tag> Taglist
        {
            set
            {
                if (value != null)
                {
                    value.ForEach(e =>
                    {
                        this.tags_.Add(e.Name, e);
                    });
                }
            }

            get
            {
                return this.tags_.Values.ToList();
            }
        }

        private Dictionary<string, Tag> tags_ = new Dictionary<string, Tag>(0);

        public int Count()
        {
            return tags_.Count;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach(Tag t in this.tags_.Values)
            {
                s.AppendLine(t.ToString());
            }
            return s.ToString();
        }
    }
}
