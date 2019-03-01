using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SendQuizComposer<T> : OutgoingHandlerPacket where T : SendQuizComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendQuiz);
            message.AppendString(handler.Name);

            message.AppendInt32(handler.Questions.Count);
            foreach(int question in handler.Questions)
            {
                message.AppendInt32(question);
            }
            return message;
        }
    }
}
