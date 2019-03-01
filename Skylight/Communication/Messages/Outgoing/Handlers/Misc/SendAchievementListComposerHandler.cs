using SkylightEmulator.HabboHotel.Users.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc
{
    public class SendAchievementListComposerHandler : OutgoingHandler
    {
        public UserAchievements UserAchievements;

        public SendAchievementListComposerHandler(UserAchievements userAchievements)
        {
            this.UserAchievements = userAchievements;
        }
    }
}
