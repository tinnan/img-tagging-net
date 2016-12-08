using static img_tagging_test.TagLibTestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using img_tagging.tag;

namespace img_tagging_test
{
    [TestClass]
    public class TagLib_RemoveTagTest
    {
        [TestMethod]
        public void TestRemoveTag_RemoveUnexistTag_NoChangeInTagsInstance()
        {
            Tags actual = GetCopyTestMockTags();
            actual.RemoveTags("ไม่มีจริง"); // try to remove unexist tag.

            Tags expected = GetCopyTestMockTags();

            Assert.AreEqual(expected, actual, "Expected no changes.");
        }

        [TestMethod]
        public void TestRemoveTag_RemoveNormalTag_TagRemovedFromTaglist()
        {
            string[] deletes = new string[] { "ทดสอบ 02" };

            Tags actual = GetCopyTestMockTags();
            actual.RemoveTags(deletes);

            Tags expected = GetCopyTestMockTags();

            Assert.IsTrue(expected.EqualsSiteList(actual), "Expected no changes in site list.");
            Assert.IsTrue(expected.EqualsActressList(actual), "Expected no changes in actress list.");

            AssertTagList(expected, actual, deletes);
        }

        [TestMethod]
        public void TestRemoveTag_RemoveSiteTag_TagRemovedFromTaglistAndSiteList()
        {
            string[] deletes = new string[] { "Site 01" };

            Tags actual = GetCopyTestMockTags();
            actual.RemoveTags(deletes);

            Tags expected = GetCopyTestMockTags();

            Assert.AreEqual(1, actual.Sites.Count, "Expected site list to remain only 1 tag.");
            Assert.IsFalse(actual.Sites.Contains(deletes[0]), "Expected to to be deleted.");
            Assert.AreEqual("Site 02", actual.Sites.ToArray()[0]);

            Assert.IsTrue(expected.EqualsActressList(actual), "Expected no changes in actress list.");

            AssertTagList(expected, actual, deletes);
        }

        [TestMethod]
        public void TestRemoveTag_RemoveActressTag_TagRemovedFromTaglistAndActressList()
        {
            string[] deletes = new string[] { "คนที่ 01" };

            Tags actual = GetCopyTestMockTags();
            actual.RemoveTags(deletes);

            Tags expected = GetCopyTestMockTags();

            Assert.IsTrue(expected.EqualsSiteList(actual), "Expected no changes in site list.");

            Assert.AreEqual(2, actual.Actresses.Count, "Expected actress list to remain only 2 tag.");
            Assert.IsFalse(actual.Actresses.Contains(deletes[0]), "Expected to to be deleted.");
            Assert.IsTrue(actual.Actresses.Contains("คนที่ 02"));
            Assert.IsTrue(actual.Actresses.Contains("คนที่ 03"));

            AssertTagList(expected, actual, deletes);
        }

        private void AssertTagList(Tags expected, Tags actual, string[] deletes)
        {
            Assert.AreEqual(expected.Count() - deletes.Length, actual.Count());
            string[] actualTaglist = actual.GetTagNames();
            string[] expectedTaglist = expected.GetTagNames();
            foreach (string delete in deletes)
            {
                Assert.IsFalse(actualTaglist.Contains(delete), "Expected tag '" + delete + "' to be deleted.");
            }

            foreach (string actualTag in actualTaglist)
            {
                Assert.IsTrue(expectedTaglist.Contains(actualTag), "Expected other tags unchanged.");
            }
        }
    }
}
