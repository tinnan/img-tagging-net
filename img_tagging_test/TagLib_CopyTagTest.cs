using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using img_tagging.tag;

namespace img_tagging_test
{
    [TestClass]
    public class TagLib_CopyTagTest
    {
        [TestMethod]
        public void TestCopyTag_DuplicateOriginAndTarget()
        {
            Tags tags = GetCopyTestMockTags();
            try
            {
                tags.CopyTag("ทดสอบ 01", "ทดสอบ 02", "ทดสอบ 01", "ทดสอบ 03");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.AreEqual("ทดสอบ 01", e.DuplicatedTag);
                return;
            }

            Assert.Fail("Expected IllegalTagCopyTargetException.");
        }

        private Tags GetCopyTestMockTags()
        {
            Tags tags = new Tags();

            tags.AddSite("Site 01");
            tags.AddSite("Site 02");

            tags.AddActress("คนที่ 01");
            tags.AddActress("คนที่ 02");
            tags.AddActress("คนที่ 03");

            List<Tag> taglist = new List<Tag>();
            Tag tag01 = new Tag("ทดสอบ 01");
            // TODO: assign members tag01
            taglist.Add(tag01);

            Tag tag02 = new Tag("ทดสอบ 02");
            // TODO: assign members tag02
            taglist.Add(tag02);

            Tag tag03 = new Tag("ทดสอบ 03");
            // TODO: assign members tag03
            taglist.Add(tag03);

            Tag site01 = new Tag("Site 01");
            site01.Type = "S";
            // TODO: assign members site01
            taglist.Add(site01);

            Tag site02 = new Tag("Site 02");
            site02.Type = "S";
            // TODO: assign members site02
            taglist.Add(site02);

            Tag acc01 = new Tag("คนที่ 01");
            acc01.Type = "A";
            // TODO: assign members acc01
            taglist.Add(acc01);

            Tag acc02 = new Tag("คนที่ 02");
            acc02.Type = "A";
            // TODO: assign members acc02
            taglist.Add(acc02);

            Tag acc03 = new Tag("คนที่ 03");
            acc03.Type = "A";
            // TODO: assign members acc03
            taglist.Add(acc03);

            tags.Taglist = taglist;

            return tags;
        }
    }
}
