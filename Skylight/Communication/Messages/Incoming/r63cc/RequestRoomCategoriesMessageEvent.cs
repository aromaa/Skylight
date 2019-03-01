using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63cc
{
    class RequestRoomCategoriesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            List<FlatCat> flatCats = Skylight.GetGame().GetNavigatorManager().GetFlatCats();

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Message.Init(r63ccOutgoing.FlatCats);
            Message.AppendInt32(flatCats.Count); //count
            foreach (FlatCat flatcat in flatCats)
            {
                Message.AppendInt32(flatcat.Id); //flat cat id
                Message.AppendString(flatcat.Caption); //name
                Message.AppendBoolean(flatcat.MinRank <= session.GetHabbo().Rank); //visible
                Message.AppendBoolean(false);
                Message.AppendString(flatcat.Caption);
                Message.AppendString("");
                Message.AppendBoolean(false);
            }
            session.SendMessage(Message);
        }
    }
}
