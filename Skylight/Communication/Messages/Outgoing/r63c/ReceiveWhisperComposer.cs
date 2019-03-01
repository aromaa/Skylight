using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class ReceiveWhisperComposer<T> : OutgoingHandlerPacket where T : ReceiveWhisperComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.ReceiveWhisper);
            message.AppendInt32(handler.SenderVirtualID);

            List<string> links = new List<string>();
            if (TextUtilies.ContainsLink(handler.Message))
            {
                string message_ = "";
                foreach (string word in handler.Message.Split(' '))
                {
                    if (TextUtilies.ValidURL(word))
                    {
                        links.Add(word);

                        message_ += " {" + (links.Count - 1) + "}";
                    }
                    else
                    {
                        message_ += " " + word;
                    }
                }

                message.AppendString(message_);
            }
            else
            {
                message.AppendString(handler.Message);
            }
            
            message.AppendInt32(RoomUnit.GetGesture(handler.Message));
            message.AppendInt32(handler.ChatColor);
            message.AppendInt32(links.Count);
            for(int i = 0; i < links.Count; i++)
            {
                string link = links[i];

                message.AppendString("/redirect.php?url=" + link); //link
                message.AppendString(link); //msg shown
                message.AppendBoolean(true); //trusted, can be clicked and opened, unused
            }
            message.AppendInt32(-1);
            return message;
        }
    }
}
