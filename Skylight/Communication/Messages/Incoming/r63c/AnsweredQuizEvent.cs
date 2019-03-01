using SkylightEmulator.Communication.Messages.Incoming.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class AnsweredQuizEvent : AnsweredQuizEventHandler
    {
        public override void Handle(GameClient session, ClientMessage message)
        {
            this.Name = message.PopFixedString();
            this.Answers = new int[message.PopWiredInt32()];
            for(int i = 0; i < this.Answers.Length; i++)
            {
                this.Answers[i] = message.PopWiredInt32();
            }

            base.Handle(session, message);
        }
    }
}
