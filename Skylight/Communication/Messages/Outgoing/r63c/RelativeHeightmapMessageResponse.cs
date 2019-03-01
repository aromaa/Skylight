using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RelativeHeightmapMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            string relativeHeightmap = valueHolder.GetValue<string>("RelativeHeightmap");
            string[] splitd = relativeHeightmap.Split(new char[] { Convert.ToChar(13) }, StringSplitOptions.RemoveEmptyEntries);
            int total = splitd.Sum(s => s.Length);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            Message.Init(r63cOutgoing.RelativeHeightmap);
            Message.AppendInt32(total / splitd.Length); //"height"
            Message.AppendInt32(total);
            foreach (string string_ in splitd)
            {
                foreach (char char_ in string_)
                {
                    ushort short_;
                    ushort.TryParse(char_.ToString(), out short_);
                    Message.AppendShort(char_.ToString().ToLower() == "x" ? 65535 : short_);
                }
            }
            return Message;
        }
    }
}
