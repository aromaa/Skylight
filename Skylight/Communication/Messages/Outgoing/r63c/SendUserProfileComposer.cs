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
    class SendUserProfileComposer<T> : OutgoingHandlerPacket where T : SendUserProfileComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendUserProfile);
            message.AppendUInt(handler.Profile.UserID);
            message.AppendString(handler.Profile.Username);
            message.AppendString(handler.Profile.Look);
            message.AppendString(handler.Profile.Motto);
            message.AppendString(TimeUtilies.UnixTimestampToDateTime(handler.Profile.AccountCreated).ToString());
            message.AppendInt32(handler.Profile.AchievementScore);
            message.AppendInt32(handler.Profile.FriendsCount);
            message.AppendBoolean(handler.FriendsWith);
            message.AppendBoolean(handler.RequestSended);
            message.AppendBoolean(handler.Profile.Online);
            message.AppendInt32(0); //groups
            message.AppendInt32((int)TimeUtilies.GetUnixTimestamp() - (int)handler.Profile.LastOnline);
            message.AppendBoolean(true); //idk
            return message;
        }
    }
}
