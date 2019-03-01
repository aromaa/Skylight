using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc
{
    public class AnsweredQuizEventHandler : IncomingPacket
    {
        private static readonly Dictionary<int, int> RightAnswers = new Dictionary<int, int>()
        {
            { 0, 2 },
            { 1, 1 },
            { 2, 2 },
            { 3, 0 },
            { 4, 1 },
            { 5, 3 },
            { 6, 1 },
            { 7, 3 },
            { 8, 0 },
            { 9, 0 },
        };

        protected string Name;
        protected int[] Answers;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo() != null)
            {
                if (this.Name == "HabboWay1")
                {
                    List<int> wrongAnswers = new List<int>();
                    for(int i = 0; i < this.Answers.Length; i++)
                    {
                        if (this.Answers[i] != AnsweredQuizEventHandler.RightAnswers[session.GetHabbo().HabboWayQuestions[i]])
                        {
                            wrongAnswers.Add(session.GetHabbo().HabboWayQuestions[i]);
                        }
                    }

                    session.SendMessage(new QuizResultsComposerHandler(this.Name, wrongAnswers));
                    session.GetHabbo().HabboWayQuestions = null;

                    if (wrongAnswers.Count <= 0)
                    {
                        session.GetHabbo().GetUserAchievements().AchievementUnlocked("HabboWayGraduate", 1);
                    }
                }
            }
        }
    }
}
