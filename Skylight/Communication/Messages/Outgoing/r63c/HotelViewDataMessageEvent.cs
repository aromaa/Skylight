using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class HotelViewDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string data = message.PopFixedString();

            foreach(string promoData in data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] data_ = promoData.Split(new char[] { ',' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2)
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
                    message_.Init(r63cOutgoing.HotelViewData);
                    message_.AppendString(data_[0]);
                    message_.AppendString(data_[1]);
                    session.SendMessage(message_);
                }
            }
        }
    }
}
