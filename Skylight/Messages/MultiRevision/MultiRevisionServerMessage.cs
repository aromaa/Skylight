using SkylightEmulator.Communication;
using SkylightEmulator.Messages.Empty;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.MultiRevision
{
    public class MultiRevisionServerMessage
    {
        private static readonly ValueHolder EmptyValueHolder = new ValueHolder();

        private ValueHolder ValueHolder;
        private Dictionary<Revision, byte[]> Bytes;
        private OutgoingPacketsEnum Packet;

        public MultiRevisionServerMessage(OutgoingPacketsEnum packet)
        {
            this.Bytes = new Dictionary<Revision, byte[]>();
            this.ValueHolder = MultiRevisionServerMessage.EmptyValueHolder;
            this.Packet = packet;
        }
        public MultiRevisionServerMessage(OutgoingPacketsEnum packet, ValueHolder valueHolder)
        {
            this.Bytes = new Dictionary<Revision, byte[]>();
            this.ValueHolder = valueHolder ?? MultiRevisionServerMessage.EmptyValueHolder;
            this.Packet = packet;
        }

        public ValueHolder GetValueHolder()
        {
            return this.ValueHolder;
        }

        public byte[] GetBytes(Revision revision)
        {
            byte[] bytes = null;
            if (!this.Bytes.TryGetValue(revision, out bytes))
            {
                OutgoingPacket packet_ = null;
                if (BasicUtilies.GetRevisionPacketManager(revision).HandleOutgoing(this.Packet, out packet_))
                {
                    ServerMessage message = packet_.Handle(this.ValueHolder);
                    if (message == null || message is EmptyServerMessage)
                    {
                        bytes = null;
                    }
                    else
                    {
                        bytes = message.GetBytes();
                    }
                }
                else
                {
                    throw new Exception("Unable to find packet id: " + this.Packet + ", from: " + revision);
                }

                this.Bytes.Add(revision, bytes);
            }

            return bytes;
        }
    }
}
