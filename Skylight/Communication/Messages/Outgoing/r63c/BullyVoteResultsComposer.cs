using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guardian;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class BullyVoteResultsComposer<T> : OutgoingHandlerPacket where T : BullyVoteResultsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.BullyVoteResults);

            message.AppendInt32(handler.Votes.Count);
            foreach(GuardianVote vote in handler.Votes)
            {
                message.AppendInt32((int)vote);
            }
            return message;
        }
    }
}
