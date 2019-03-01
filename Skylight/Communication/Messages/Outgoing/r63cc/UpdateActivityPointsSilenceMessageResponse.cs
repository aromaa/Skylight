using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class UpdateActivityPointsSilenceMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            Dictionary<int, int> currnecy = valueHolder.GetValue<Dictionary<int, int>>("ActivityPoints");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Message.Init(r63ccOutgoing.UpdateActivityPointsSilence);
            Message.AppendInt32(currnecy.Count); //count
            foreach (KeyValuePair<int, int> data in currnecy)
            {
                Message.AppendInt32(data.Key); //id
                Message.AppendInt32(data.Value); //amount
            }
            return Message;
        }
    }
}
