using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class FlatCatsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<FlatCat> flatCats = valueHolder.GetValue<List<FlatCat>>("FlatCats");
            int rank = valueHolder.GetValue<int>("Rank");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            Message.Init(r63cOutgoing.FlatCats);
            Message.AppendInt32(flatCats.Count); //count
            foreach (FlatCat flatcat in flatCats)
            {
                Message.AppendInt32(flatcat.Id); //flat cat id
                Message.AppendString(flatcat.Caption); //name
                Message.AppendBoolean(flatcat.MinRank <= rank); //visible
                Message.AppendBoolean(false);
                Message.AppendString(flatcat.Caption);
                Message.AppendString("");
                Message.AppendBoolean(false);
            }
            return Message;
        }
    }
}
