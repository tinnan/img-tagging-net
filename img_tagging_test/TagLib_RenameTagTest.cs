using static img_tagging_test.TagLibTestUtils;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using img_tagging.tag;
using System.Linq;

namespace img_tagging_test
{
    [TestClass]
    public class TagLib_RenameTagTest
    {
        public TagLib_RenameTagTest()
        {
        }

        [TestMethod]
        public void TestRenameTag_SameOriginAndTargetTag_DuplicatedTagCopyException()
        {
            string origin = "Site 01";
            string target = origin;

            Tags actual = GetCopyTestMockTags();

            try
            {
                actual.RenameTag(origin, target);
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                return;
            }

            Assert.Fail("Expected DuplicatedTagCopyTargetException.");
        }

        [TestMethod]
        public void TestRenameTag_RenameFromNonexistTag_NothingChanged()
        {
            string origin = "Non exist";
            string target = "Tag 01";

            Tags actual = GetCopyTestMockTags();
            Tags expected = GetCopyTestMockTags();

            try
            {
                Tag newTag = actual.RenameTag(origin, target);

                Assert.IsNull(newTag);

                Assert.AreEqual(expected, actual, "Expected no changes.");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }

        [TestMethod]
        public void TestRenameTag_RenameToNewNormalTag_NewTagCreatedSamePropNoChangeSiteOrActressList()
        {
            string origin = "ทดสอบ 01";
            string target = "newtag 01";

            Tags actual = GetCopyTestMockTags();
            Tags expected = GetCopyTestMockTags();

            try
            {
                Tag newTag = actual.RenameTag(origin, target);

                // Assert no changes.
                Assert.IsTrue(expected.EqualsSiteList(actual), "Expected no change in site list.");
                Assert.IsTrue(expected.EqualsActressList(actual), "Expected no change in actress list.");

                // Assert new tag.
                Assert.IsNotNull(newTag);
                Assert.IsTrue(actual.ContainsTag(target), "Expected target exist in tag list.");

                Tag expectedTag = expected.GetOrCreateTag(origin);

                Assert.AreEqual(target, newTag.Name);
                Assert.IsTrue(expectedTag.EqualsType(newTag), "Expected same tag type.");
                Assert.IsTrue(expectedTag.EqualsDescription(newTag), "Expected same description.");
                Assert.IsTrue(expectedTag.EqualsMembers(newTag), "Expected same members.");

                // Assert remove original tag.
                Assert.IsFalse(actual.ContainsTag(origin), "Expected original tag to be removed from tag list.");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }

        [TestMethod]
        public void TestRenameTag_RenameToNewSiteTag_NewTagCreatedSamePropChangeSiteList()
        {
            string origin = "Site 01";
            string target = "newtag 01";

            Tags actual = GetCopyTestMockTags();
            Tags expected = GetCopyTestMockTags();

            try
            {
                Tag newTag = actual.RenameTag(origin, target);

                // Assert no changes.
                Assert.IsTrue(expected.EqualsActressList(actual), "Expected no change in actress list.");

                // Assert new tag.
                Assert.IsNotNull(newTag);
                Assert.IsTrue(actual.ContainsTag(target), "Expected target exist in tag list.");

                Assert.IsTrue(actual.Sites.Contains(target), "Expected new tag in site list.");

                Tag expectedTag = expected.GetOrCreateTag(origin);

                Assert.AreEqual(target, newTag.Name);
                Assert.IsTrue(expectedTag.EqualsType(newTag), "Expected same tag type.");
                Assert.IsTrue(expectedTag.EqualsDescription(newTag), "Expected same description.");
                Assert.IsTrue(expectedTag.EqualsMembers(newTag), "Expected same members.");

                // Assert remove original tag.
                Assert.IsFalse(actual.Sites.Contains(origin), "Expected original tag to be removed from site list.");
                Assert.IsFalse(actual.ContainsTag(origin), "Expected original tag to be removed from tag list.");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }

        [TestMethod]
        public void TestRenameTag_RenameToNewActressTag_NewTagCreatedSamePropChangeActressList()
        {
            string origin = "คนที่ 01";
            string target = "newtag 01";

            Tags actual = GetCopyTestMockTags();
            Tags expected = GetCopyTestMockTags();

            try
            {
                Tag newTag = actual.RenameTag(origin, target);

                // Assert no changes.
                Assert.IsTrue(expected.EqualsSiteList(actual), "Expected no change in site list.");

                // Assert new tag.
                Assert.IsNotNull(newTag);
                Assert.IsTrue(actual.ContainsTag(target), "Expected target exist in tag list.");

                Assert.IsTrue(actual.Actresses.Contains(target), "Expected new tag in actress list.");

                Tag expectedTag = expected.GetOrCreateTag(origin);

                Assert.AreEqual(target, newTag.Name);
                Assert.IsTrue(expectedTag.EqualsType(newTag), "Expected same tag type.");
                Assert.IsTrue(expectedTag.EqualsDescription(newTag), "Expected same description.");
                Assert.IsTrue(expectedTag.EqualsMembers(newTag), "Expected same members.");

                // Assert remove original tag.
                Assert.IsFalse(actual.Actresses.Contains(origin), "Expected original tag to be removed from actress list.");
                Assert.IsFalse(actual.ContainsTag(origin), "Expected original tag to be removed from tag list.");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }

        [TestMethod]
        public void TestRenameTag_RenameToExistingActressTag_NoNewTagNoChangeToPropSiteAndActressList()
        {
            string origin = "ทดสอบ 01";
            string target = "คนที่ 01";

            Tags actual = GetCopyTestMockTags();
            Tags expected = GetCopyTestMockTags();
            string[] expectedMembers = expected.GetOrCreateTag(origin).Members
                .Union(expected.GetOrCreateTag(target).Members).ToArray();

            try
            {
                Tag newTag = actual.RenameTag(origin, target);

                // Assert no changes.
                Assert.IsTrue(expected.EqualsSiteList(actual), "Expected no change in site list.");
                Assert.IsTrue(expected.EqualsActressList(actual), "Expected no change in actress list.");

                // Assert new tag.
                Assert.IsNull(newTag);

                // Assert existing target tag.
                Tag expectedTag = expected.GetOrCreateTag(target);
                Tag actualTag = actual.GetOrCreateTag(target);

                Assert.AreEqual(target, actualTag.Name);
                Assert.IsTrue(expectedTag.EqualsType(actualTag), "Expected no change in tag type.");
                Assert.IsTrue(expectedTag.EqualsDescription(actualTag), "Expected no change in description.");

                string[] actualMembers = actualTag.Members.ToArray();
                Assert.AreEqual(expectedMembers.Length, actualMembers.Length);
                foreach (string actualMember in actualMembers)
                {
                    Assert.IsTrue(expectedMembers.Contains(actualMember));
                }
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }
    }
}
