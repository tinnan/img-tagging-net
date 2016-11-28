using img_tagging.tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace img_tagging_test
{
    sealed class TagLibTestUtils
    {
        private TagLibTestUtils()
        {

        }

        public static Tags GetCopyTestMockTags()
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

        public static string[] GetMemberSet_01()
        {
            return new string[] { "pic0001", "pic0002", "pic0003", "pic0004" };
        }

        public static string[] GetMemberSet_02()
        {
            return new string[] { "pic0002", "pic0004", "pic0005", "pic0006", "pic0007" };
        }

        public static string[] GetMemberSet_03()
        {
            return new string[] { "pic0004", "pic0005", "pic0007", "pic0008", "pic0009", "pic0010" };
        }

        public static string[] GetMemberSet_04()
        {
            return new string[] { "pic0010", "pic0007", "pic0001", "pic0006", "pic0011", "pic0012" };
        }

        public static string[] GetMemberSet_05()
        {
            return new string[] { "pic0005", "pic0002", "pic0013", "pic0014", "pic0001" };
        }

        public static string[] GetMemberSet_06()
        {
            return new string[] { "pic0004", "pic0007", "pic0010", "pic0011", "pic0015", "pic0016" };
        }

        public static ISet<string> JoinUniqueMembers(params string[][] members)
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
