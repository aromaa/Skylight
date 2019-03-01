using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Wardrobe;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SendWardrobeComposer<T> : OutgoingHandlerPacket where T : SendWardrobeComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendWardrobe);
            message.AppendInt32(1); //state, unused
            message.AppendInt32(handler.Slots.Count);
            foreach(WardrobeSlot slot in handler.Slots)
            {
                message.AppendInt32(slot.SlotID);
                message.AppendString(slot.Look);
                message.AppendString(slot.Gender);
            }
            return message;
        }
    }
}
