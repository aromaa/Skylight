using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class PublicItemsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message_2.Init(r26Outgoing.PublicItems);
            string[][] items = valueHolder.GetValueOrDefault<string[][]>("Items");
            if (items != null && items.Length > 0)
            {
                foreach (string[] item in items)
                {
                    message_2.AppendString(item[0]);
                    message_2.AppendString(item[1]);
                    message_2.AppendString(item[2]);
                    message_2.AppendString(item[3]);
                    message_2.AppendString(item[4]);
                    message_2.AppendString(item[5], 13);
                }
            }
            return message_2;
        }
    }
}
