using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class PublicItemsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage publicItems = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            publicItems.Init(r63aOutgoing.PublicItems);

            string[][] items = valueHolder.GetValueOrDefault<string[][]>("Items");
            if (items != null && items.Length > 0)
            {
                publicItems.AppendInt32(items.Length);

                foreach(string[] item in items)
                {
                    publicItems.AppendBoolean(false);
                    publicItems.AppendString(item[0]);
                    publicItems.AppendString(item[1]);
                    publicItems.AppendInt32(int.Parse(item[2]));
                    publicItems.AppendInt32(int.Parse(item[3]));
                    publicItems.AppendInt32(int.Parse(item[4]));
                    publicItems.AppendInt32(int.Parse(item[5]));
                }
            }
            else
            {
                publicItems.AppendInt32(0);
            }
            return publicItems;
        }
    }
}
