using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Newtonsoft.Json;
using img_tagging.tag;
using System.Collections.Generic;
using img_tagging.tag.converter;

namespace img_tagging_test
{
    [TestClass]
    public class TagLibLoadingTest
    {
        [TestMethod]
        public void TestLoadingFromJSON()
        {
            StringBuilder s = new StringBuilder();
            s.Append("{")
                .Append("\"Sites\": [ \"siamdara.com\", \"darapic.com\" ],")
                .Append("\"Actresses\": [ \"ขวัญ\", \"แตงโม\" ],")
                .Append("\"Taglist\": [{")
                .Append("\"Name\": \"น่ารัก\",")
                .Append("\"Description\": \"Cute\",")
                .Append("\"Members\": [ \"0132545.jpg\", \"45774124.png\" ]")
                .Append("}, {")
                .Append("\"Name\": \"sjsjsj\",")
                .Append("\"Description\": \"nmansd\",")
                .Append("\"Members\": [ \"4478514_7s.jpg\", \"7064547d5.png\" ]")
                .Append("}]")
                .Append("}");
            
            Tags t = JsonConvert.DeserializeObject<Tags>(s.ToString(), new TagsConverter());

            // assert
            Assert.IsNotNull(t); // Tags is successfully converted.

            // assert Sites
            Assert.IsNotNull(t.Sites);
            Assert.IsTrue(t.Sites.Contains("siamdara.com") && t.Sites.Contains("darapic.com"));

            // assert Actresses
            Assert.IsNotNull(t.Actresses);
            Assert.IsTrue(t.Actresses.Contains("ขวัญ") && t.Actresses.Contains("แตงโม"));

            // assert Tag list
            Assert.IsNotNull(t.Taglist);
            List<Tag> taglist = t.Taglist;
            // assert first tag set
            Tag t1 = taglist[0];
            Assert.AreEqual("น่ารัก", t1.Name);
            Assert.AreEqual("Cute", t1.Description);
            // assert first tag set members
            Assert.IsNotNull(t1.Members);
            string[] t1_member = new string[t1.Members.Count];
            t1.Members.CopyTo(t1_member);
            Assert.AreEqual("0132545.jpg", t1_member[0]);
            Assert.AreEqual("45774124.png", t1_member[1]);

            // assert second tag set
            Tag t2 = taglist[1];
            Assert.AreEqual("sjsjsj", t2.Name);
            Assert.AreEqual("nmansd", t2.Description);
            // assert second tag set members
            Assert.IsNotNull(t2.Members);
            string[] t2_member = new string[t2.Members.Count];
            t2.Members.CopyTo(t2_member);
            Assert.AreEqual("4478514_7s.jpg", t2_member[0]);
            Assert.AreEqual("7064547d5.png", t2_member[1]);
        }
    }
}
