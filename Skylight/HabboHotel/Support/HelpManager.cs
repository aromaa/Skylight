using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class HelpManager
    {
        private Dictionary<int, FAQ> FAQs;

        public HelpManager()
        {
            this.FAQs = new Dictionary<int, FAQ>();
        }

        public void LoadFAQs(DatabaseClient dbClient)
        {
            Logging.Write("Loading FAQs... ");
            this.FAQs.Clear();

            DataTable faq = dbClient.ReadDataTable("SELECT * FROM help_faq");
            if (faq != null && faq.Rows.Count > 0)
            {
                foreach(DataRow dataRow in faq.Rows)
                {
                    int id = (int)dataRow["id"];
                    this.FAQs.Add(id, new FAQ(id, (string)dataRow["subject"], (string)dataRow["title"], (string)dataRow["body"], int.Parse(dataRow["known_issue"].ToString())));
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public ServerMessage GetFAQs()
        {
            List<FAQ> issues = this.FAQs.Values.Where(f => f.KnownIssue == 2).ToList();
            List<FAQ> issues2 = this.FAQs.Values.Where(f => f.KnownIssue == 1).ToList();

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.FAQs);
            message.AppendInt32(issues.Count);
            foreach(FAQ faq in issues)
            {
                message.AppendInt32(faq.ID);
                message.AppendString(faq.Title);
            }

            message.AppendInt32(issues2.Count);
            foreach(FAQ faq in issues2)
            {
                message.AppendInt32(faq.ID);
                message.AppendString(faq.Title);
            }

            return message;
        }

        public ServerMessage GetAllFAQs()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.AllFAQs);
            message.AppendInt32(this.FAQs.Values.GroupBy(f => f.Subject).Count());
            foreach(IGrouping<string, FAQ> faqGroup in this.FAQs.Values.GroupBy(f => f.Subject))
            {
                foreach (FAQ faq in faqGroup)
                {
                    message.AppendInt32(faq.ID);
                    message.AppendString(faq.Subject);
                    message.AppendInt32(faqGroup.Count());

                    break;
                }
            }
            return message;
        }

        public ServerMessage GetCategorys(int categoryId)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.FAQCategorys);
            message.AppendInt32(categoryId);
            message.AppendString(""); //desc

            FAQ faq = null;
            if (this.FAQs.TryGetValue(categoryId, out faq))
            {
                List<FAQ> categorsys = this.FAQs.Values.Where(f => f.Subject == faq.Subject).ToList();

                message.AppendInt32(categorsys.Count);
                foreach(FAQ category in categorsys)
                {
                    message.AppendInt32(category.ID);
                    message.AppendString(category.Title);
                }
            }
            else
            {
                message.AppendInt32(0);
            }

            return message;
        }

        public ServerMessage GetFAQ(int faqId)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.FAQ);
            message.AppendInt32(faqId);

            FAQ faq = null;
            if (this.FAQs.TryGetValue(faqId, out faq))
            {
                message.AppendString(faq.Body);
            }
            else
            {
                message.AppendString("BODY NOT FOUND");
            }

            return message;
        }

        public void Shutdown()
        {
            if (this.FAQs != null)
            {
                this.FAQs.Clear();
            }
            this.FAQs = null;
        }
    }
}
