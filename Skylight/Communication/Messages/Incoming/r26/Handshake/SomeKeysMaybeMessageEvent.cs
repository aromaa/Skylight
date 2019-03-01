using SkylightEmulator.Communication.Handlers;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Encryption;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class SomeKeysMaybeMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null)
            {
                //string test = message.PopFixedString();

                //ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                //Message.Init(r26Outgoing.SomeKeysMaybe);
                //Message.AppendString("QBHIIIKHJIPAIQAdd-MM-yyyy");
                //Message.AppendString("SAHPB/client");
                //Message.AppendString("QBH" + "IJWVVVSNKQCFUBJASMSLKUUOJCOLJQPNSBIRSVQBRXZQOTGPMNJIHLVJCRRULBLUO", null);
                //session.SendMessage(Message);
                
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                Message.Init(1);
                Message.AppendString("0", null);
                session.SendMessage(Message);

                session.GetDataHandler<OldCryptoDataDecoderHandler>(OldCryptoDataDecoderHandler.Identifier_).SetRC4(new RC4(new int[] { 0 }));
            }
        }
    }
}
