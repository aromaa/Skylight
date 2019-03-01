using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class UpdateRoomUserComposerHandler : OutgoingHandler
    {
        public int VirtualID { get; }
        public string Look { get; }
        public string Gender { get; }
        public string Motto { get; }
        public int AchievmentScore { get; }

        public UpdateRoomUserComposerHandler(int virtualId, string look, string gender, string motto, int achievementScore)
        {
            this.VirtualID = virtualId;
            this.Look = look;
            this.Gender = gender;
            this.Motto = motto;
            this.AchievmentScore = achievementScore;
        }
    }
}
