using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class LatencyTestReportMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int averageLatency = message.PopWiredInt32(); //latency total / extends count
            int lowestLatency = message.PopWiredInt32(); //latency < averageLatency * 2
            int extendsCount = message.PopWiredInt32();

            if (Skylight.GetConfig()["client.ping.limit.enabled"] == "1")
            {
                if (averageLatency > int.Parse(Skylight.GetConfig()["client.ping.limit"]))
                {
                    session.Stop("Too high latency!");
                }
            }
        }
    }
}
