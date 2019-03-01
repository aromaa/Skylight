using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class FAQ
    {
        public readonly int ID;
        public readonly string Subject;
        public readonly string Title;
        public readonly string Body;
        public readonly int KnownIssue;

        public FAQ(int id, string subject, string title, string body, int knownIssue)
        {
            this.ID = id;
            this.Subject = subject;
            this.Title = title;
            this.Body = body;
            this.KnownIssue = knownIssue;
        }
    }
}
