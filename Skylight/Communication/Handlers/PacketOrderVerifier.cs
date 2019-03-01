using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Handlers
{
    public class PacketOrderVerifier : MessageHandler
    {
        public static readonly Guid DefaultIdentifier = Guid.Parse("2d90e570-ddd5-4ec5-99e1-b508f17bd320");

        private readonly Guid CurrentIdentifier = PacketOrderVerifier.DefaultIdentifier;
        public Queue<uint> ExceptedPacketOrder;

        public PacketOrderVerifier()
        {
            this.ExceptedPacketOrder = new Queue<uint>();
        }

        public override Guid Identifier()
        {
            return this.CurrentIdentifier;
        }

        public override bool HandleMessage(GameClient session, ClientMessage message)
        {
            if (this.ExceptedPacketOrder.Count > 0)
            {
                if (message.GetID() != r63aIncoming.Pong) //this dcs some people randomly
                {
                    uint exceptedId = this.ExceptedPacketOrder.Dequeue();
                    if (exceptedId == message.GetID())
                    {
                        return true;
                    }
                    else
                    {
                        Logging.WriteLine("Client violated packet order! Excepted: " + exceptedId + " | Received: " + message.GetID());
                        session.Stop("Invalid packet order!");
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PacketOrderVerifier)
            {
                return ((PacketOrderVerifier)obj).Identifier() == this.Identifier();
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.CurrentIdentifier.GetHashCode();
        }
    }
}
