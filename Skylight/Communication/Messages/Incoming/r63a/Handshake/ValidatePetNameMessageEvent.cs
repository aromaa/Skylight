using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ValidatePetNameMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string name = message.PopFixedString();
            int type = message.PopWiredInt32();

            if (name.Length >= 2)
            {
                if (name.Length <= 16)
                {
                    Regex regex = new Regex(@"^[A-Z0-9_-]+$", RegexOptions.IgnoreCase);
                    foreach (char char_ in name)
                    {
                        if (!regex.IsMatch(char_.ToString()))
                        {
                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            message_.Init(r63aOutgoing.ValidatePetNameResult);
                            message_.AppendInt32(3); //0 = valid, 1 = too long, 2 = too short, 3 = forbidden cheracters, 4 = forbidden words
                            message_.AppendString(char_.ToString());
                            session.SendMessage(message_);

                            return; //we don't want continue :(
                        }
                    }

                    if (TextUtilies.HaveBlacklistedWords(name))
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidatePetNameResult);
                        message_.AppendInt32(4); //0 = valid, 1 = too long, 2 = too short, 3 = forbidden cheracters, 4 = forbidden words
                        message_.AppendString(""); //show the forbidden word
                        session.SendMessage(message_);
                    }
                    else //let the user buy it
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidatePetNameResult);
                        message_.AppendInt32(0); //0 = valid, 1 = too long, 2 = too short, 3 = forbidden cheracters, 4 = forbidden words
                        message_.AppendString(""); //show the forbidden word
                        session.SendMessage(message_);
                    }
                }
                else
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.ValidatePetNameResult);
                    message_.AppendInt32(1); //0 = valid, 1 = too long, 2 = too short, 3 = forbidden cheracters, 4 = forbidden words
                    message_.AppendString("16");
                    session.SendMessage(message_);
                }
            }
            else
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.ValidatePetNameResult);
                message_.AppendInt32(2); //0 = valid, 1 = too long, 2 = too short, 3 = forbidden cheracters, 4 = forbidden words
                message_.AppendString("2");
                session.SendMessage(message_);
            }
        }
    }
}
