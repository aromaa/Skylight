using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using SkylightEmulator.Utilies;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class CompletedQuizEventHandler : IncomingPacket
    {
        protected string Type;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                if (this.Type == "SafetyQuiz1")
                {
                    session.GetHabbo().GetUserAchievements().AchievementUnlocked("SafetyQuiz", 1);
                }
                else if (this.Type == "HabboWay1")
                {
                    HashSet<int> questions = new HashSet<int>();
                    while (questions.Count < 5)
                    {
                        questions.Add(RandomUtilies.GetRandom(0, 9));
                    }

                    session.GetHabbo().HabboWayQuestions = questions.ToList();
                    session.SendMessage(new SendQuizComposerHandler("HabboWay1", questions));
                }
            }
        }
    }
}
