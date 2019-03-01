using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class VariablesMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int number = message.PopWiredInt32();
            if (number == 401)
            {
                string swfFolderLocation = message.PopFixedString();
                string externalVariablesLocation = message.PopFixedString();
            }
            else
            {
                session.Stop("Invalid variables");
            }
        }
    }
}
