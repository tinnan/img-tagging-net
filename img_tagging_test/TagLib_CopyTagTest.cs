using static img_tagging_test.TagLibTestUtils;
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
        public void TestCopyTag_InvalidOriginType_InvalidOriginTagException()
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
        public void TestCopyTag_InvalidTargetType_InvalidTagTypeException()
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
        public void TestCopyTag_DuplicateOriginAndTarget_DuplicatedTagCopyException()
        {
            Tags tags = GetCopyTestMockTags();
            try
            {
                tags.CopyTag("คนที่ 99", "ทดสอบ 02", "คนที่ 99", "ทดสอบ 03");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.AreEqual("คนที่ 99", e.DuplicatedTag);
                return;
            }

            Assert.Fail("Expected IllegalTagCopyTargetException.");
        }

        [TestMethod]
        public void TestCopyTag_NonExisitingOrigin_NoChangeInTagsInstance()
        {
            Tags tags = GetCopyTestMockTags();

            Tag[] newTags = new Tag[0];
            try
            {
                newTags = tags.CopyTag("ไม่มีจริง", "ทดสอบ 02", "ทดสอบ 03", "แท็กใหม่");
            }
            catch (Exception e)
            {
                Assert.Fail("Not expecting any exception.");
            }

            Assert.AreEqual(0, newTags.Length, "Expected no new tag.");

            Assert.AreEqual(GetCopyTestMockTags(), tags, "Expected no changes."); 
        }

        [TestMethod]
        public void TestCopyTag_CopyToExistingTag_TagInfoCopiedToExistingTag()
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

                    if (target01.Equals(tagName)
                        || target02.Equals(tagName))
                    {
                        
                        ISet<string> expectedMembers;

                        if (target01.Equals(tagName))
                        {
                            // Set 01 + Set 06
                            expectedMembers = JoinUniqueMembers(GetMemberSet_01(), GetMemberSet_06());
                        }
                        else
                        {
                            // Set 06 + Set 05
                            expectedMembers = JoinUniqueMembers(GetMemberSet_05(), GetMemberSet_06());
                        }

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

        [TestMethod]
        public void TestCopyTag_CopyToNewTag_NewTagIsCreated()
        {
            Tags tags = GetCopyTestMockTags();

            string ori = "คนที่ 03"; // Set 06
            string target01 = "แท็กใหม่ 01"; // New tag

            Tag[] newTags = new Tag[0];
            try
            {
                newTags = tags.CopyTag(ori, target01);
            }
            catch (Exception e)
            {
                Assert.Fail("Not expecting any exception.");
            }

            Assert.AreEqual(1, newTags.Length, "Expected 1 new tag.");

            Tags expectedTags = GetCopyTestMockTags();

            Assert.IsTrue(tags.EqualsSiteList(expectedTags), "Expected unchanged site list.");
            Assert.IsTrue(tags.EqualsActressList(expectedTags), "Expected unchanged actress list.");
            Assert.IsTrue(expectedTags.Taglist.Count < tags.Taglist.Count, "Expected larger tag member size in result tag.");

            foreach (Tag expectedTag in expectedTags.Taglist) // Assert the unchanged.
            {
                string tagName = expectedTag.Name;

                Tag actualTag = tags.GetOrCreateTag(tagName);

                if (!target01.Equals(tagName))
                {
                    Assert.AreEqual(expectedTag, actualTag, "Expected no changes.");
                }
            }

            // Assert new tag.
            Tag newTag = tags.GetOrCreateTag(target01); // Get new tag by name.
            Assert.AreEqual(target01, newTag.Name);
            Assert.AreEqual(TagType.T.ToString(), newTag.Type);
            Assert.AreEqual(null, newTag.Description);
            string[] expectedMembers = GetMemberSet_06(); // Set 06
            ISet<string> actualMembers = newTag.Members;
            Assert.AreEqual(expectedMembers.Length, actualMembers.Count);
            foreach (string expectedMember in expectedMembers)
            {
                Assert.IsTrue(actualMembers.Contains(expectedMember), "Expected to contain the same member.");
            }

            // Compare with returned value.
            Assert.AreEqual(newTags[0], newTag);
        }
    }
}
