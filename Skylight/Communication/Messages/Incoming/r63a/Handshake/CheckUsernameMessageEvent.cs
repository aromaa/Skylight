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
    class CheckUsernameMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.FlagmeCommandUsed)
            {
                if (session.GetHabbo().HasPermission("cmd_flagme"))
                {
                    string username = TextUtilies.FilterString(message.PopFixedString());
                    if (username.Length < 3) //to short
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(2); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else if (username.Length > 15) // too long
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(3); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else if (username.Contains(" ") || !Regex.IsMatch(username, "^[-a-zA-Z0-9._:,]+$") || TextUtilies.HaveBlacklistedWords(username)) //invalid name
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(4); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else if (Skylight.GetGame().GetGameClientManager().UsernameExits(username)) //name already exits
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(5); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                    else
                    {
                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.ValidUsername);
                        message_.AppendInt32(0); //result
                        message_.AppendString(username);
                        message_.AppendInt32(0); //suggested names
                        session.SendMessage(message_);
                    }
                }
            }
            else
            {

            }
        }
    }
}
