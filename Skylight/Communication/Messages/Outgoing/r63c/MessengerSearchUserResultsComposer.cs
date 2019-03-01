using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using System.Data;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    public class MessengerSearchUserResultsComposer<T> : OutgoingHandlerPacket where T : MessengerSearchUserResultsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        private ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerUserSearchResults);
            message.AppendInt32(handler.Friends.Count);
            foreach(DataRow user in handler.Friends)
            {
                this.WriteUser(message, user);
            }

            message.AppendInt32(handler.Users.Count);
            foreach(DataRow dataRow in handler.Users)
            {
                this.WriteUser(message, dataRow);
            }
            return message;
        }

        private void WriteUser(ServerMessage message, DataRow user)
        {
            message.AppendUInt((uint)user["id"]);
            message.AppendString((string)user["username"]);
            message.AppendString((string)user["motto"]);
            message.AppendBoolean((string)user["online"] == "1"); //is online, unused
            message.AppendBoolean(false); //in room, unused
            message.AppendString(""); //last online, unused
            message.AppendInt32(0); //category, unused
            message.AppendString((string)user["look"]);
            message.AppendString(""); //real name, unused
        }
    }
}
