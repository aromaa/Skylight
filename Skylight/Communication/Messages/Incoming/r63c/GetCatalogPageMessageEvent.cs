using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class GetCatalogPageMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            CatalogPage page = Skylight.GetGame().GetCatalogManager().GetCatalogPage(message.PopWiredInt32());
            if (page != null && page.Visible && page.Enabled && page.MinRank <= session.GetHabbo().Rank)
            {
                session.SendData(page.GetBytes(session.Revision));
            }
        }
    }
}
