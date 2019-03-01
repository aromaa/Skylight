using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    public class ClientMessageHandler : MessageHandler
    {
        public static readonly ClientMessageHandler Handler = new ClientMessageHandler();
        private readonly Guid Identifier_ = Guid.Parse("59f29f5d-7f28-489d-983b-eaee935d352a");

        public override Guid Identifier()
        {
            return this.Identifier_;
        }

        public override bool HandleMessage(GameClient session, ClientMessage message)
        {
            IncomingPacket incomingPacket;
            if (BasicUtilies.GetRevisionPacketManager(session.Revision).HandleIncoming(message.GetID(), out incomingPacket))
            {
                incomingPacket.Handle(session, message);

                try
                {
                    if (Skylight.GetConfig()["debug.incoming"] == "1")
                    {
                        Logging.WriteLine(string.Concat(new object[]
                        {
                            "[",
                            session.ID,
                            "] --> [",
                            message.GetID(),
                            "] ",
                            message.GetHeader(),
                            message.GetBody()
                        }));
                    }
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    if (Skylight.GetConfig()["show.unhandled.packets"] == "1")
                    {
                        Logging.WriteLine(string.Concat(new object[]
                        {
                            "[",
                            session.ID,
                            "] --> Packet dosen't exit: [",
                            message.GetID(),
                            "] ",
                            message.GetHeader(),
                            message.GetBody()
                        }));
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientMessageHandler)
            {
                return ((ClientMessageHandler)obj).Identifier() == this.Identifier();
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Identifier_.GetHashCode();
        }
    }
}
