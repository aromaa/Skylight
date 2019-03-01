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
    class SendMySettingsComposer<T> : OutgoingHandlerPacket where T : SendMySettingsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendMySettings);
            message.AppendInt32(handler.VolumeSystem);
            message.AppendInt32(handler.VolumeFurni);
            message.AppendInt32(handler.VolumeTrax);
            message.AppendBoolean(handler.PreferOldChat);
            message.AppendBoolean(handler.BlockRoomInvites);
            message.AppendBoolean(handler.BlockCameraFollow);
            message.AppendInt32(1);
            message.AppendInt32(handler.ChatColor); //chat color
            return message;
        }
    }
}
