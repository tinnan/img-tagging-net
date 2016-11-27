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
        public void TestCopyTag_InvalidOriginType()
        {
            Tags tags = GetCopyTestMockTags();
            try
            {
                tags.CopyTag("Site 01", "ทดสอบ 02", "คนที่ 01", "ทดสอบ 03");
            }
            catch (InvalidOriginTagTypeException e)
            {
                return;
            }

            Assert.Fail("Expected InvalidOriginTagTypeException.");
        }

        [TestMethod]
        public void TestCopyTag_InvalidTargetType()
        {
            Tags tags = GetCopyTestMockTags();
            try
            {
                tags.CopyTag("คนที่ 01", "ทดสอบ 02", "Site 01", "ทดสอบ 03");
            }
            catch (InvalidTargetTagTypeException e)
            {
                return;
            }
            
            Assert.Fail("Expected InvalidTargetTagTypeException.");
        }

        [TestMethod]
        public void TestCopyTag_DuplicateOriginAndTarget()
        {
            Tags tags = GetCopyTestMockTags();
            try
            {
                tags.CopyTag("คนที่ 01", "ทดสอบ 02", "คนที่ 01", "ทดสอบ 03");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.AreEqual("คนที่ 01", e.DuplicatedTag);
                return;
            }

            Assert.Fail("Expected IllegalTagCopyTargetException.");
        }

        [TestMethod]
        public void TestCopyTag_NonExisitingOrigin()
        {
            Tags tags = GetCopyTestMockTags();

            Tag[] newTags = new Tag[0];
            try
            {
                newTags = tags.CopyTag("ไม่มีจริง", "ทดสอบ 02", "คนที่ 01", "ทดสอบ 03", "แท็กใหม่");
            }
            catch (Exception e)
            {
                Assert.Fail("Not expecting any exception.");
            }

            Assert.AreEqual(0, newTags.Length, "Expected no new tag.");

            Assert.AreEqual(GetCopyTestMockTags(), tags, "Expected no changes."); 
        }

        [TestMethod]
        public void TestCopyTag_CopyToExistingTag()
        {
            Tags tags = GetCopyTestMockTags();

            string ori = "คนที่ 03"; // Set 06
            string target01 = "ทดสอบ 01"; // Set 01
            string target02 = "ทดสอบ 03"; // Set 05

            Tag[] newTags = new Tag[0];
            try
            {
                newTags = tags.CopyTag(ori, target01, target02);
            }
            catch (Exception e)
            {
                Assert.Fail("Not expecting any exception.");
            }

            Assert.AreEqual(0, newTags.Length, "Expected no new tag.");

            Tags expectedTags = GetCopyTestMockTags();

            Assert.IsTrue(tags.EqualsSiteList(expectedTags), "Expected unchanged site list.");
            Assert.IsTrue(tags.EqualsActressList(expectedTags), "Expected unchanged actress list.");
            Assert.AreEqual(expectedTags.Taglist.Count, tags.Taglist.Count, "Expected same number of tag.");

            foreach(Tag expectedTag in expectedTags.Taglist)
            {
                string tagName = expectedTag.Name;

                Tag actualTag = tags.GetOrCreateTag(tagName);

                if (target01.Equals(tagName) 
                    || target02.Equals(tagName))
                {
                    Assert.IsTrue(expectedTag.EqualsType(actualTag), "Expected type is not changed.");
                    Assert.IsTrue(expectedTag.EqualsDescription(actualTag), "Expected description is not changed.");

                    if (target01.Equals(tagName))
                    {
                        // Set 01 + Set 06
                        ISet<string> expectedMembers = JoinUniqueMembers(GetMemberSet_01(), GetMemberSet_06());
                        ISet<string> actualMembers = actualTag.Members;

                        Assert.AreEqual(expectedMembers.Count, actualMembers.Count, "Expected same member count.");
                        foreach (string expectedMember in expectedMembers)
                        {
                            Assert.IsTrue(actualMembers.Contains(expectedMember), "Expected to contain the same member.");
                        }
                    }

                } else
                {
                    Assert.AreEqual(expectedTag, actualTag, "Expected no changes.");
                }
            }
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
            tag01.AddMembers(GetMemberSet_01()); // Set 01
            taglist.Add(tag01);

            Tag tag02 = new Tag("ทดสอบ 02");
            tag02.AddMembers(GetMemberSet_02()); // Set 02
            taglist.Add(tag02);

            Tag tag03 = new Tag("ทดสอบ 03");
            tag03.AddMembers(GetMemberSet_05()); // Set 05
            taglist.Add(tag03);

            Tag site01 = new Tag("Site 01");
            site01.Type = "S";
            site01.AddMembers(GetMemberSet_03()); // Set 03
            taglist.Add(site01);

            Tag site02 = new Tag("Site 02");
            site02.Type = "S";
            site02.AddMembers(GetMemberSet_04()); // Set 04
            taglist.Add(site02);

            Tag acc01 = new Tag("คนที่ 01");
            acc01.Type = "A";
            acc01.AddMembers(GetMemberSet_03()); // Set 03
            taglist.Add(acc01);

            Tag acc02 = new Tag("คนที่ 02");
            acc02.Type = "A";
            acc02.AddMembers(GetMemberSet_04()); // Set 04
            taglist.Add(acc02);

            Tag acc03 = new Tag("คนที่ 03");
            acc03.Type = "A";
            acc03.AddMembers(GetMemberSet_06()); // Set 06
            taglist.Add(acc03);

            tags.Taglist = taglist;

            return tags;
        }

        private string[] GetMemberSet_01()
        {
            return new string[] { "pic0001", "pic0002", "pic0003", "pic0004" };
        }

        private string[] GetMemberSet_02()
        {
            return new string[] { "pic0002", "pic0004", "pic0005", "pic0006", "pic0007" };
        }

        private string[] GetMemberSet_03()
        {
            return new string[] { "pic0004", "pic0005", "pic0007", "pic0008", "pic0009", "pic0010" };
        }

        private string[] GetMemberSet_04()
        {
            return new string[] { "pic0010", "pic0007", "pic0001", "pic0006", "pic0011", "pic0012" };
        }

        private string[] GetMemberSet_05()
        {
            return new string[] { "pic0005", "pic0002", "pic0013", "pic0014", "pic0001" };
        }

        private string[] GetMemberSet_06()
        {
            return new string[] { "pic0004", "pic0007", "pic0010", "pic0011", "pic0015", "pic0016" };
        }

        private ISet<string> JoinUniqueMembers(params string[][] members)
        {
            List<string> memberList = new List<string>();
            foreach (string[] member in members)
            {
                memberList.AddRange(member);
            }
            return new HashSet<string>(memberList);
        }
    }
}
