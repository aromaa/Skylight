using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class NewNavigatorMetaDataMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            message.Init(r63ccOutgoing.NewNavigatorMetaData);
            message.AppendInt32(4); //count

            message.AppendString("official_view");
            message.AppendInt32(0); //saved search
            /*message.AppendInt32(1); //saved search
            message.AppendInt32(1); //ID
            message.AppendString("adw"); //code, IDK
            message.AppendString("adw"); //filter, IDK
            message.AppendString("adw"); //localization, IDK*/


            message.AppendString("hotel_view");
            message.AppendInt32(0); //saved search

            message.AppendString("roomads_view");
            message.AppendInt32(0); //saved search

            message.AppendString("myworld_view");
            message.AppendInt32(0); //saved search
            return message;
        }
    }
}
