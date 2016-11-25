using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace img_tagging.tag
{
    public class Tags
    {
        public Tags()
        {
            Sites = new HashSet<string>();
            Actresses = new HashSet<string>();
            Taglist = new List<Tag>(0);
        }
        
        public HashSet<string> Sites { set; get; }
        public HashSet<string> Actresses { set; get; }
        public List<Tag> Taglist
        {
            set
            {
                if (value != null)
                {
                    value.ForEach(e =>
                    {
                        this._tags.Add(e.Name, e);
                    });
                }
            }

            get
            {
                return this._tags.Values.ToList();
            }
        }

        private Dictionary<string, Tag> _tags = new Dictionary<string, Tag>(0);

        public int Count()
        {
            return _tags.Count;
        }

        public bool AddSite(string site)
        {
            return Sites.Add(site);
        }

        public bool AddActress(string actress)
        {
            return Actresses.Add(actress);
        }

        /// <summary>
        /// Get Tag from _tags with the specified name.
        /// If the tag does not exist, a new one is created and returned.
        /// </summary>
        /// <param name="tname">Tag name.</param>
        /// <returns>Existing tag or new.</returns>
        public Tag GetOrCreateTag(string tname)
        {
            if(_tags.ContainsKey(tname))
            {
                return _tags[tname];
            }

            Tag t = new Tag(tname);
            _tags.Add(tname, t);
            return t;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach(Tag t in this._tags.Values)
            {
                s.AppendLine(t.ToString());
            }
            return s.ToString();
        }

        /// <summary>
        /// Merge tag(s) to destination tag, copy all their members to destination tag
        /// and Remove themselves from tag list.
        /// </summary>
        /// <param name="from">Tag being renamed</param>
        /// <param name="to">Destination tag</param>
        public void MergeTag(string[] from, string to)
        {

        }
    }
}
