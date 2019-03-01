using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class MachineIdMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string machineId = message.PopFixedString();
            if (machineId.Length <= 2)
            {
                session.Stop("OPZ");
            }

            //Guid guid;
            //if (!Guid.TryParse(machineId.Split(':')[0], out guid))
            //{
            //    string idByte = "";
            //    foreach (byte byte_ in TimeUtilies.GetUnixTimestamp().ToString())
            //    {
            //        idByte += byte_;
            //    }

            //    long ipId = 0;
            //    foreach (byte byte_ in session.GetIP().ToString())
            //    {
            //        ipId += (int)byte_;
            //    }

            //    machineId = Guid.NewGuid() + ":" + idByte + ":" + ipId;

            //    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            //    message_.Init(r26Outgoing.MachineID);
            //    message_.AppendString(machineId);
            //    session.SendMessage(message_);
            //}

            session.MachineID = machineId;
        }
    }
}
