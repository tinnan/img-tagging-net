using static img_tagging_test.TagLibTestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using img_tagging.tag;

namespace img_tagging_test
{
    [TestClass]
    public class TagLib_MergeTagTest
    {

        [TestMethod]
        public void TestMergeTag_DuplicateOriginAndTarget_DuplicatedTagCopyTargetException()
        {
            string[] origins = new string[] { "ทดสอบ 01", "ทดสอบ 02" };
            string target = "ทดสอบ 01";

            Tags actualTags = GetCopyTestMockTags();

            try
            {
                actualTags.MergeTag(target, origins);
            } catch (DuplicatedTagCopyTargetException e)
            {
                return;
            }

            Assert.Fail("Expected DuplicatedTagCopyTargetException.");
        }

        [TestMethod]
        public void TestMergeTag_NonexistOrigins_NothingChanged()
        {
            string[] origins = new string[] { "nonexist 01", "nonexist 02" };
            string target = "ทดสอบ 01";

            Tags actualTags = GetCopyTestMockTags();
            Tags expectedTags = GetCopyTestMockTags();

            try
            {
                Tag newTag = actualTags.MergeTag(target, origins);

                Assert.IsNull(newTag);
                Assert.AreEqual(expectedTags, actualTags, "Expected no changes.");
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }

        [TestMethod]
        public void TestMergeTag_MergeToNewTag_NewTagCreatedAsNormalType()
        {
            string ori_site = "Site 01";
            string ori_actress = "คนที่ 03";
            string ori_normal = "ทดสอบ 02";
            string target = "newtag 01";

            Tags actualTags = GetCopyTestMockTags();
            Tags expectedTags = GetCopyTestMockTags();

            string[] expectedMembers = expectedTags.GetOrCreateTag(ori_site).Members
                .Union(expectedTags.GetOrCreateTag(ori_actress).Members)
                .Union(expectedTags.GetOrCreateTag(ori_normal).Members).ToArray();

            try
            {
                Tag newTag = actualTags.MergeTag(target, ori_site, ori_actress, ori_normal);

                // Assert new tag.
                Assert.IsNotNull(newTag);
                Assert.IsTrue(actualTags.ContainsTag(target));

                Assert.AreEqual(target, newTag.Name); // Name.
                Assert.AreEqual("T", newTag.Type); // Type is normal.
                Assert.IsNull(newTag.Description); // Description is null.
                Assert.AreEqual(expectedMembers.Length, newTag.Members.Count); // Member count.
                foreach (string actualMember in newTag.Members)
                {
                    Assert.IsTrue(expectedMembers.Contains(actualMember)); // Membership is expected.
                }

                // Assert removed tags.
                Assert.IsFalse(actualTags.Sites.Contains(ori_site)); // Original site tag is removed.
                Assert.IsFalse(actualTags.Actresses.Contains(ori_actress)); // Original actress tag is removed.

                Assert.IsFalse(actualTags.ContainsTag(ori_site));
                Assert.IsFalse(actualTags.ContainsTag(ori_actress));
                Assert.IsFalse(actualTags.ContainsTag(ori_normal));
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }

        [TestMethod]
        public void TestMergeTag_MergeToExistingTag_NoNewTagChangeOnlyMembers()
        {
            string ori_1 = "ทดสอบ 01";
            string ori_2 = "ทดสอบ 03";
            string target = "ทดสอบ 02";

            Tags actualTags = GetCopyTestMockTags();
            Tags expectedTags = GetCopyTestMockTags();

            string[] expectedMembers = expectedTags.GetOrCreateTag(ori_1).Members
                .Union(expectedTags.GetOrCreateTag(ori_2).Members)
                .Union(expectedTags.GetOrCreateTag(target).Members).ToArray();

            try
            {
                Tag newTag = actualTags.MergeTag(target, ori_1, ori_2);

                Assert.IsTrue(expectedTags.EqualsSiteList(actualTags)); // No change in site list.
                Assert.IsTrue(expectedTags.EqualsActressList(actualTags)); // No change in actress list.

                Assert.IsNull(newTag); // No new tag created.

                Tag expectedTag = expectedTags.GetOrCreateTag(target);
                Tag actualTag = actualTags.GetOrCreateTag(target);
                Assert.AreEqual(target, actualTag.Name); // Name.
                Assert.AreEqual(expectedTag.Type, actualTag.Type); // Type.
                Assert.AreEqual(expectedTag.Description, actualTag.Description); // Description.
                Assert.AreEqual(expectedMembers.Length, actualTag.Members.Count); // Member count.
                foreach (string actualMember in actualTag.Members)
                {
                    Assert.IsTrue(expectedMembers.Contains(actualMember)); // Membership is expected.
                }

                // Assert originals are removed.
                Assert.IsFalse(actualTags.ContainsTag(ori_1));
                Assert.IsFalse(actualTags.ContainsTag(ori_2));
            }
            catch (DuplicatedTagCopyTargetException e)
            {
                Assert.Fail("Not expecting any exception.");
            }
        }
    }
}
