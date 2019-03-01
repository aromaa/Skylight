using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class MachineIdIncomingMessage : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string machineId = message.PopFixedString();
            string newMachineId = message.PopFixedString(); //We don't care about this
            string someData = message.PopFixedString();

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

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
                message_.Init(r63cOutgoing.MachineID);
                message_.AppendString(machineId);
                session.SendMessage(message_);
            }

            session.MachineID = machineId;
        }
    }
}
