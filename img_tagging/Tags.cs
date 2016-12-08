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

        public string[] GetTagNames()
        {
            return _tags.Keys.ToArray();
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
        /// Destination tag being created here is treated as normal tag (type=T).
        /// Origin tag that does not actually exist is ignored.
        /// </summary>
        /// <param name="dest">Destination tag</param>
        /// <param name="mergefrom">Tag(s) being merged</param>
        /// <returns>Newly created tag(s), empty if none were created.</returns>
        /// <exception cref="DuplicatedTagCopyTargetException">If there exist a member of <paramref name="dest"/> in <paramref name="mergefrom"/></exception>
        public Tag[] MergeTag(string dest, params string[] mergefrom)
        {
            bool copyType = false;
            bool copyDesc = false;

            Tag[] newTags = CopyTag(mergefrom, new string[] { dest }, copyType, copyDesc);

            // Remove origin tags from tag list.
            RemoveTags(mergefrom);

            return newTags;
        }

        /// <summary>
        /// Copy all memebers of a tag to any new or existing tag(s).
        /// New tag(s) are treated as normal tag (type=T).
        /// Origin tag that does not actually exist is ignored.
        /// Allow to operate on original tag of type A only.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Newly created tag(s), empty if none were created.</returns>
        /// <exception cref="DuplicatedTagCopyTargetException">If there exist a member of <paramref name="from"/> in <paramref name="to"/></exception>
        /// <exception cref="InvalidOriginTagTypeException">The original tag is not of type A</exception>
        /// <exception cref="InvalidTargetTagTypeException">The target tags are not of type T</exception>
        public Tag[] CopyTag(string from, params string[] to)
        {
            ValidateCopyTagType(from, to);

            bool copyType = false;
            bool copyDesc = false;

            Tag[] newTags = CopyTag(new string[] { from }, to, copyType, copyDesc);

            return newTags;
        }

        /// <summary>
        /// Rename an existing tag to a new or an existing tag name. 
        /// New tag is created with same type and description as it origin.
        /// Origin tag that does not actually exist is ignored.
        /// </summary>
        /// <param name="from">Tag to be renamed</param>
        /// <param name="to">New tag name</param>
        /// <returns>New tag instance, <code>null</code> if none were created</returns>
        /// <exception cref="DuplicatedTagCopyTargetException">If <paramref name="from"/> and <paramref name="to"/> are the same</exception>
        public Tag RenameTag(string from, string to)
        {
            bool copyType = true;
            bool copyDesc = true;

            Tag[] newTags = CopyTag(new string[] { from }, new string[] { to }, copyType, copyDesc);

            // remove original tag.
            RemoveTags(new string[] { from });

            return newTags.Length == 0 ? null : newTags[0];
        }

        /// <summary>
        /// Copy members of tags in <paramref name="from"/> to new or existing tags in <paramref name="to"/>.
        /// </summary>
        /// <param name="from">Origins tags</param>
        /// <param name="to">Destination tags</param>
        /// <param name="copyType">Flag indicated tag type copying</param>
        /// <param name="copyDesc">Flag indicated tag description copying</param>
        /// <returns>Newly created tag(s), empty if none were created.</returns>
        /// <exception cref="DuplicatedTagCopyTargetException">If there exist a member of <paramref name="from"/> in <paramref name="to"/></exception>
        private Tag[] CopyTag(string[] from, string[] to, bool copyType, bool copyDesc)
        {
            ValidateCopyTarget(from, to);

            string[] validOrigins = GetValidOrigins(from);
            if (validOrigins.Length == 0) // There is no valid origin tags, end the process now.
            {
                return new Tag[0];
            }

            IList<Tag> newTags = new List<Tag>(0);

            foreach (string f in from)
            {
                foreach (string t in to)
                {
                    Tag newTag = CopyTag(f, t, copyType, copyDesc);
                    if (newTag != null)
                    {
                        newTags.Add(newTag);
                    }
                }
            }

            return newTags.ToArray();
        }

        /// <summary>
        /// Copy members of tag <paramref name="from"/> to new or existing tag <paramref name="to"/>.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="copyType">Flag indicated tag type copying</param>
        /// <param name="copyDesc">Flag indicated tag description copying</param>
        /// <returns>Newly created tag, <code>null</code> if none were created.</returns>
        private Tag CopyTag(string from, string to, bool copyType, bool copyDesc)
        {
            bool newTag = false;
            Tag toTag;

            if (_tags.ContainsKey(to))
            {
                toTag = _tags[to];
            }
            else
            {
                newTag = true;
                toTag = new Tag(to);
                _tags.Add(to, toTag);
            }

            Tag fromTag = _tags[from];

            toTag.AddMembers(fromTag.Members.ToArray());
            if (newTag)
            {
                if (copyType)
                {
                    string type = fromTag.Type;
                    toTag.Type = type;

                    // If it is a new tag, 
                    // consider adding it to Site list or Actress list too.

                    switch (Tag.ToTagTypeEnum(type))
                    {
                        case TagType.S:
                            AddSite(to);
                            break;
                        case TagType.A:
                            AddActress(to);
                            break;
                        default:
                            break;
                    }

                }

                if (copyDesc)
                {
                    toTag.Description = fromTag.Description;
                }
            }

            return newTag ? toTag : null;
        }

        /// <summary>
        /// Remove specified tags from tag list. 
        /// And if they exist in site list or actress list, remove them from the list too.
        /// </summary>
        /// <param name="tags">Tags to be removed</param>
        public void RemoveTags(params string[] tags)
        {
            foreach (string tagName in tags)
            {
                if (_tags.ContainsKey(tagName))
                {
                    _tags.Remove(tagName);
                }

                if (Sites.Contains(tagName))
                {
                    Sites.Remove(tagName);
                }

                if (Actresses.Contains(tagName))
                {
                    Actresses.Remove(tagName);
                }
            }
        }

        /// <summary>
        /// Validate the merging/copying origins and destinations.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="DuplicatedTagCopyTargetException">If there exist a member of <paramref name="from"/> in <paramref name="to"/></exception>
        private void ValidateCopyTarget(string[] from, string[] to)
        {
            ISet<string> t_list = new HashSet<string>(to);
            foreach (string f in from)
            {
                if (t_list.Contains(f))
                {
                    throw new DuplicatedTagCopyTargetException(f);
                }
            }
        }

        /// <summary>
        /// Validate copying origin.
        /// </summary>
        /// <param name="from"></param>
        /// <exception cref="InvalidOriginTagTypeException">The original tag is not of type A</exception>
        /// <exception cref="InvalidTargetTagTypeException">The target tag is not of type T</exception>
        private void ValidateCopyTagType(string from, params string[] to)
        {
            if (_tags.ContainsKey(from))
            {
                Tag tag = _tags[from];
                if (Tag.ToTagTypeEnum(tag.Type) != TagType.A)
                {
                    throw new InvalidOriginTagTypeException();
                }
            }

            foreach (string t in to)
            {
                if (_tags.ContainsKey(t))
                {
                    Tag tag = _tags[t];
                    if (Tag.ToTagTypeEnum(tag.Type) != TagType.T)
                    {
                        throw new InvalidTargetTagTypeException();
                    }
                }
            }
            
        }

        private string[] GetValidOrigins(params string[] uncheckedtags)
        {
            HashSet<string> validOrigins = new HashSet<string>();
            foreach (string u in uncheckedtags)
            {
                if (_tags.ContainsKey(u))
                {
                    validOrigins.Add(u);
                }
            }
            return validOrigins.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Tags))
            {
                return false;
            }

            Tags that = (Tags)obj;

            // compare Site list.
            if (!EqualsSiteList(that))
            {
                return false;
            }

            // compare Actress list.
            if (!EqualsActressList(that))
            {
                return false;
            }

            // compare Tag list.
            if (!EqualsTagList(that))
            {
                return false;
            }

            // At last.
            return true;
        }

        public bool EqualsSiteList(Tags that)
        {
            if (that == null)
            {
                return false;
            }

            ISet<string> thisSites = Sites;
            ISet<string> thatSites = that.Sites;
            if (thisSites.Count != thatSites.Count)
            {
                return false;
            }

            foreach (string s in thisSites)
            {
                if (!thatSites.Contains(s))
                {
                    return false;
                }
            }

            // at last.
            return true;
        }

        public bool EqualsActressList(Tags that)
        {
            if (that == null)
            {
                return false;
            }

            ISet<string> thisActresses = Actresses;
            ISet<string> thatActresses = that.Actresses;
            if (thisActresses.Count != thatActresses.Count)
            {
                return false;
            }

            foreach (string a in thisActresses)
            {
                if (!thatActresses.Contains(a))
                {
                    return false;
                }
            }

            // at last.
            return true;
        }

        public bool EqualsTagList(Tags that)
        {
            if (that == null)
            {
                return false;
            }

            IList<Tag> thatTaglist = that.Taglist;
            if (thatTaglist.Count != _tags.Count)
            {
                return false;
            }

            foreach (Tag thatTag in thatTaglist)
            {
                string thatTagName = thatTag.Name;
                if (!_tags.ContainsKey(thatTagName))
                {
                    return false;
                }

                Tag thisTag = _tags[thatTagName];

                if (!thisTag.Equals(thatTag))
                {
                    return false;
                }
            }

            // at last.
            return true;
        }
    }

    public class DuplicatedTagCopyTargetException : Exception
    {
        public string DuplicatedTag { get; set; }

        public DuplicatedTagCopyTargetException(string tagname) 
            : base("Merging/copying/renaming the same tag is not allowed. Tag: " + tagname)
        {
            DuplicatedTag = tagname;
        }
    }

    public class InvalidOriginTagTypeException : Exception
    {
        public InvalidOriginTagTypeException() 
            : base("Only allow original tag of type A.")
        {

        }
    }

    public class InvalidTargetTagTypeException : Exception
    {
        public InvalidTargetTagTypeException()
            : base("Only allow target tag of type T.")
        {

        }
    }
}
