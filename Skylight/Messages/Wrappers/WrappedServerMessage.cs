using SkylightEmulator.Messages.Cached;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Messages.Wrappers
{
    public class WrappedServerMessage
    {
        private MultiRevisionServerMessage multi;
        private CachedServerMessage single;

        public WrappedServerMessage(MultiRevisionServerMessage multi)
        {
            this.multi = multi;
        }

        public WrappedServerMessage(ServerMessage single)
        {
            this.single = new CachedServerMessage(single);
        }
        public WrappedServerMessage(CachedServerMessage single)
        {
            this.single = single;
        }

        public bool IsMulti()
        {
            return this.multi != null;
        }

        public byte[] GetBytes()
        {
            if (!this.IsMulti())
            {
                return this.single.GetBytes();
            }
            else
            {
                return null;
            }
        }

        public byte[] GetBytes(Revision revision)
        {
            if (this.IsMulti())
            {
                return this.multi.GetBytes(revision);
            }
            else
            {
                if (this.single.GetRevision() == revision)
                {
                    return this.single.GetBytes();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
