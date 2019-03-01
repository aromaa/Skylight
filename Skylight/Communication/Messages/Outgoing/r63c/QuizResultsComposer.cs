using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class QuizResultsMessage<T> : OutgoingHandlerPacket where T : QuizResultsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.QuizResults);
            message.AppendString(handler.Name);

            message.AppendInt32(handler.WrongAnswers.Count);
            foreach(int wrongAnswer in handler.WrongAnswers)
            {
                message.AppendInt32(wrongAnswer);
            }
            return message;
        }
    }
}
