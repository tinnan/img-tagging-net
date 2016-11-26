using System;
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
        /// and remove themselves from tag list.
        /// The destination tag is created if it does not exist already.
        /// Used in the scenario of merging and renaming tag.
        /// Destination tag being created here is treated as normal tag (type=T).
        /// </summary>
        /// <param name="dest">Destination tag</param>
        /// <param name="mergefrom">Tag(s) being merged</param>
        /// <returns>Newly created tag(s), empty if none were created.</returns>
        /// <exception cref="IllegalTagCopyTargetException">If there exist a member of <paramref name="dest"/> in <paramref name="mergefrom"/></exception>
        public Tag[] MergeTag(string dest, params string[] mergefrom)
        {
            ValidateCopyTarget(new string[] { dest }, mergefrom);
            return new Tag[0];
        }

        /// <summary>
        /// Copy all memebers of a tag to any new or existing tag(s).
        /// New tag(s) are treated as normal tag (type=T).
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Newly created tag(s), empty if none were created.</returns>
        /// <exception cref="IllegalTagCopyTargetException">If there exist a member of <paramref name="from"/> in <paramref name="to"/></exception>
        public Tag[] CopyTag(string from, params string[] to)
        {
            ValidateCopyTarget(new string[] { from }, to);
            return new Tag[0];
        }

        /// <summary>
        /// Validate the merging/copying origins and destinations.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="IllegalTagCopyTargetException">If there exist a member of <paramref name="from"/> in <paramref name="to"/></exception>
        private void ValidateCopyTarget(string[] from, string[] to)
        {
            ISet<string> t_list = new HashSet<string>(to);
            foreach (string f in from)
            {
                if (t_list.Contains(f))
                {
                    throw new IllegalTagCopyTargetException(f);
                }
            }
        }
    }

    public class IllegalTagCopyTargetException : Exception
    {
        public IllegalTagCopyTargetException(string tagname) 
            : base("Merging or copying members of the same tag is not allowed. Tag: " + tagname)
        {
            
        }
    }
}
