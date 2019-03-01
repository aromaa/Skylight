using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Navigator;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class GetFlatCatsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            IEnumerable<FlatCat> flatCats = Skylight.GetGame().GetNavigatorManager().GetFlatCats().Where(f => f.MinRank <= session.GetHabbo().Rank);

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            Message.Init(r26Outgoing.FlatCats);
            Message.AppendInt32(flatCats.Count());
            foreach (FlatCat flatcat in flatCats)
            {
                Message.AppendInt32(flatcat.Id);
                Message.AppendString(flatcat.Caption);
            }
            session.SendMessage(Message);
        }
    }
}
