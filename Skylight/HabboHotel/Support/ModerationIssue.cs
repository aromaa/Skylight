using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class ModerationIssue
    {
        public string Issue;
        public string Solution;

        public ModerationIssue(string issue, string solution)
        {
            this.Issue = issue;
            this.Solution = solution;
        }
    }
}
