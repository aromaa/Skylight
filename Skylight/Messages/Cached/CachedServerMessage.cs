using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.Cached
{
    public class CachedServerMessage : ServerMessage
    {
        private ServerMessage message;
        private byte[] cachedBytes;

        public CachedServerMessage(ServerMessage message)
        {
            this.message = message;
        }

        public override void Init()
        {
            this.message.Init();
        }

        public override void Init(uint id)
        {
            this.message.Init(id);
        }

        public override uint GetID()
        {
            return this.message.GetID();
        }

        public override int GetLenght()
        {
            return this.message.GetLenght();
        }

        public override Revision GetRevision()
        {
            return this.message.GetRevision();
        }

        public override void AppendShort(int s)
        {
            this.message.AppendShort(s);
        }

        public override void AppendInt32(int i)
        {
            this.message.AppendInt32(i);
        }

        public override void AppendUInt(uint u)
        {
            this.message.AppendUInt(u);
        }

        public override void AppendString(string s)
        {
            this.message.AppendString(s);
        }

        public override void AppendString(string s, byte? b)
        {
            this.message.AppendString(s, b);
        }

        public override void AppendBoolean(bool b)
        {
            this.message.AppendBoolean(b);
        }

        public override void AppendBytes(List<byte> bytes)
        {
            this.message.AppendBytes(bytes);
        }

        public override void AppendBytes(byte[] bytes)
        {
            this.message.AppendBytes(bytes);
        }

        public override byte[] GetBytes()
        {
            if (this.cachedBytes != null)
            {
                return this.cachedBytes;
            }

            return (this.cachedBytes = this.message.GetBytes());
        }

        public override string GetHeader()
        {
            return this.message.GetHeader();
        }

        public override string GetBody()
        {
            return this.message.GetBody();
        }
    }
}
