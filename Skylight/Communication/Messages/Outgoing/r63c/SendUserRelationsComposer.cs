using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Profiles;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SendUserRelationsComposer<T> : OutgoingHandlerPacket where T : SendUserRelationsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendUserRelations);
            message.AppendUInt(handler.Profile.UserID);

            int total = 0;
            if (handler.Profile.Lovers.Count > 0 | handler.Profile.Friends.Count > 0 | handler.Profile.Haters.Count > 0)
            {
                total++;
            }

            message.AppendInt32(total);
            if (handler.Profile.Lovers.Count > 0)
            {
                message.AppendInt32(1);
                message.AppendInt32(handler.Profile.Lovers.Count);

                UserProfile random = Skylight.GetGame().GetUserProfileManager().GetProfile(handler.Profile.Lovers[RandomUtilies.GetRandom(0, handler.Profile.Lovers.Count - 1)]);
                message.AppendUInt(random.UserID);
                message.AppendString(random.Username);
                message.AppendString(random.Look);
            }

            if (handler.Profile.Friends.Count > 0)
            {
                message.AppendInt32(2);
                message.AppendInt32(handler.Profile.Friends.Count);

                UserProfile random = Skylight.GetGame().GetUserProfileManager().GetProfile(handler.Profile.Friends[RandomUtilies.GetRandom(0, handler.Profile.Friends.Count - 1)]);
                message.AppendUInt(random.UserID);
                message.AppendString(random.Username);
                message.AppendString(random.Look);
            }

            if (handler.Profile.Haters.Count > 0)
            {
                message.AppendInt32(3);
                message.AppendInt32(handler.Profile.Haters.Count);

                UserProfile random = Skylight.GetGame().GetUserProfileManager().GetProfile(handler.Profile.Haters[RandomUtilies.GetRandom(0, handler.Profile.Haters.Count - 1)]);
                message.AppendUInt(random.UserID);
                message.AppendString(random.Username);
                message.AppendString(random.Look);
            }
            return message;
        }
    }
}
