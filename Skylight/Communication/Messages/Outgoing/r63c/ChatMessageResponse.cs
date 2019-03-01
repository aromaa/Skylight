using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class ChatMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            string message = valueHolder.GetValue<string>("Message");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            Message.Init(r63cOutgoing.Say);
            Message.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            Dictionary<int, string> links = new Dictionary<int, string>();
            if (message.Contains("http://") || message.Contains("www.") || message.Contains("https://"))
            {
                string[] words = message.Split(' ');
                message = "";

                foreach (string word in words)
                {
                    if (TextUtilies.ValidURL(word))
                    {
                        int index = links.Count;
                        links.Add(index, word);

                        message += " {" + index + "}";
                    }
                    else
                    {
                        message += " " + word;
                    }
                }
            }
            Message.AppendString(message);
            Message.AppendInt32(RoomUnit.GetGesture(message.ToLower()));
            Message.AppendInt32(valueHolder.GetValueOrDefault<int>("Bubble"));
            Message.AppendInt32(links.Count); //links count
            foreach (KeyValuePair<int, string> link in links)
            {
                Message.AppendString("/redirect.php?url=" + link.Value);
                Message.AppendString(link.Value);
                Message.AppendBoolean(true); //trushed, can link be opened
            }
            Message.AppendInt32(-1); //unknown
            return Message;
        }
    }
}
