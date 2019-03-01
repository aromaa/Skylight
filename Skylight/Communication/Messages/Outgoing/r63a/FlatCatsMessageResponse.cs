using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class FlatCatsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<FlatCat> flatCats = valueHolder.GetValue<List<FlatCat>>("FlatCats");
            int rank = valueHolder.GetValue<int>("Rank");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.FlatCats);
            Message.AppendInt32(flatCats.Count); //count
            foreach (FlatCat flatcat in flatCats)
            {
                Message.AppendInt32(flatcat.Id); //flat cat id
                Message.AppendString(flatcat.Caption); //name
                Message.AppendBoolean(flatcat.MinRank <= rank); //visible
            }
            return Message;
        }
    }
}
