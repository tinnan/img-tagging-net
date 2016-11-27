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
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>Newly created tag(s), empty if none were created.</returns>
        /// <exception cref="DuplicatedTagCopyTargetException">If there exist a member of <paramref name="from"/> in <paramref name="to"/></exception>
        public Tag[] CopyTag(string from, params string[] to)
        {
            bool copyType = false;
            bool copyDesc = false;

            Tag[] newTags = CopyTag(new string[] { from }, to, copyType, copyDesc);

            return newTags;
        }

        /// <summary>
        /// Rename an existing tag. New tag name must not exist in the tag list.
        /// Origin tag that does not actually exist is ignored.
        /// </summary>
        /// <param name="from">Tag to be renamed</param>
        /// <param name="to">New tag name</param>
        /// <returns>New tag instance, <code>null</code> if none were created</returns>
        /// <exception cref="UnexistTagRenameException">If <paramref name="from"/> does not exist in tag list</exception>
        /// <exception cref="DuplicatedTagCopyTargetException">If <paramref name="from"/> and <paramref name="to"/> are the same</exception>
        public Tag RenameTag(string from, string to)
        {
            ValidateRenameOrigin(from);

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
            } else
            {
                newTag = true;
                toTag = new Tag(to);
            }

            Tag fromTag = _tags[from];

            toTag.AddMembers(fromTag.Members.ToArray());
            if (copyType)
            {
                string type = fromTag.Type;
                toTag.Type = type;

                // If it is a new tag, 
                // consider adding it to Site list or Actress list too.
                if (newTag)
                {
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
            }

            if (copyDesc)
            {
                toTag.Description = fromTag.Description;
            }
            
            return newTag ? toTag : null;
        }

        private void RemoveTags(params string[] tags)
        {
            foreach (string t in tags)
            {
                if (_tags.ContainsKey(t))
                {
                    _tags.Remove(t);
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
        /// Validate renameing origin.
        /// </summary>
        /// <param name="from"></param>
        /// <exception cref="UnexistTagRenameException">If original tag does not exist in tag list</exception>
        private void ValidateRenameOrigin(string from)
        {
            if (!_tags.ContainsKey(from))
            {
                throw new UnexistTagRenameException(from);
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

    public class UnexistTagRenameException : Exception
    {
        public string UnexistTag { get; set; }

        public UnexistTagRenameException(string tagname) 
            : base("Renaming error: original tag [" + tagname + "] does not exist in tag list.")
        {
            UnexistTag = tagname;
        }
    }
}
