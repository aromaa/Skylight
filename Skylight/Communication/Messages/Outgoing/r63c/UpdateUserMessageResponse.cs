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
    class UpdateUserMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.UpdateUser);
            message_.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            message_.AppendString(valueHolder.GetValue<string>("Look"));
            message_.AppendString(valueHolder.GetValue<string>("Gender").ToLower());
            message_.AppendString(valueHolder.GetValue<string>("Motto"));
            message_.AppendInt32(valueHolder.GetValue<int>("AchievementPoints"));
            return message_;
        }
    }
}
