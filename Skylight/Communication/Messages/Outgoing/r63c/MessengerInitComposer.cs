using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class MessengerInitComposer<T> : OutgoingHandlerPacket where T : MessengerInitComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerInit);
            message.AppendInt32(handler.Limit);
            message.AppendInt32(0); //unused
            message.AppendInt32(handler.HCLimit);
            message.AppendInt32(handler.VIPLimit);

            message.AppendInt32(handler.Categorys.Count);
            foreach(MessengerCategory category in handler.Categorys)
            {
                message.AppendInt32(category.Id);
                message.AppendString(category.Name);
            }
            return message;
        }
    }
}
