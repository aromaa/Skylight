using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class UserAchievementScoreComposerHandler : OutgoingHandler
    {
        public int AchievementScore { get; }

        public UserAchievementScoreComposerHandler(int achievementScore)
        {
            this.AchievementScore = achievementScore;
        }
    }
}
