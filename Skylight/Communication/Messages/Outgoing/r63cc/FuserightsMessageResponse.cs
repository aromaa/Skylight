using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class FuserightsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            GameClient session = valueHolder.GetValue<GameClient>("Session");

            ServerMessage Fuserights = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Fuserights.Init(r63ccOutgoing.Fuserights);
            if (session.GetHabbo().IsVIP() || ServerConfiguration.EveryoneVIP)
            {
                Fuserights.AppendInt32(2);
            }
            else
            {
                if (session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_club"))
                {
                    Fuserights.AppendInt32(1);
                }
                else
                {
                    Fuserights.AppendInt32(0);
                }
            }

            if (session.GetHabbo().HasPermission("acc_anyroomowner"))
            {
                Fuserights.AppendInt32(7);
            }
            else
            {
                if (session.GetHabbo().HasPermission("acc_anyroomrights"))
                {
                    Fuserights.AppendInt32(5);
                }
                else
                {
                    if (session.GetHabbo().HasPermission("acc_supporttool"))
                    {
                        Fuserights.AppendInt32(4);
                    }
                    else
                    {
                        if (ServerConfiguration.EveryoneVIP || session.GetHabbo().IsVIP() || session.GetHabbo().GetSubscriptionManager().HasSubscription("habbo_club"))
                        {
                            Fuserights.AppendInt32(2);
                        }
                        else
                        {
                            Fuserights.AppendInt32(0);
                        }
                    }
                }
            }
            Fuserights.AppendBoolean(session.GetHabbo().HasPermission("acc_ambassador"));
            return Fuserights;
        }
    }
}
