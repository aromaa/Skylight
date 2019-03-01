using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Catalog;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class GetCatalogPageMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string[] data = message.PopStringUntilBreak(null).Split('/');

            CatalogPage page = Skylight.GetGame().GetCatalogManager().GetCatalogPage(int.Parse(data[1]));
            if (page != null && page.Visible && page.Enabled && page.MinRank <= session.GetHabbo().Rank)
            {
                session.SendData(page.GetBytes(session.Revision));
            }
        }
    }
}
