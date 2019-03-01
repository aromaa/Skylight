using SkylightEmulator.Communication.Handlers;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class MachineIdMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string machineId = message.PopFixedString();

            Guid guid;
            if (!Guid.TryParse(machineId.Split(':')[0], out guid))
            {
                string idByte = "";
                foreach (byte byte_ in TimeUtilies.GetUnixTimestamp().ToString())
                {
                    idByte += byte_;
                }

                long ipId = 0;
                foreach (byte byte_ in session.GetIP().ToString())
                {
                    ipId += (int)byte_;
                }

                machineId = Guid.NewGuid() + ":" + idByte + ":" + ipId;

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.MachineID);
                message_.AppendString(machineId);
                session.SendMessage(message_);
            }

            session.MachineID = machineId;
        }
    }
}
