using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Messenger
{
    public class MessengerFriend
    {
        public readonly uint ID;
        public readonly string Username;
        public readonly string Look;
        public readonly string Motto;
        public readonly double LastLogin;
        public bool NeedUpdate = false;

        public bool IsOnline
        {
            get
            {
                GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.ID);
                return gameClient != null && !gameClient.Disconnected && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null && !gameClient.GetHabbo().GetUserSettings().HideOnline;
            }
        }

        public bool InRoom
        {
            get
            {
                GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.ID);
                return gameClient != null && !gameClient.Disconnected && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetRoomSession() != null && gameClient.GetHabbo().GetRoomSession().IsInRoom && !gameClient.GetHabbo().GetUserSettings().HideInRoom; 
            }
        }

        public MessengerFriend(uint id, string look, string motto, double lastLogin)
        {
            this.ID = id;
            this.Username = Skylight.GetGame().GetGameClientManager().GetUsernameByID(id);
            this.Look = look;
            this.Motto = motto;
            this.LastLogin = lastLogin;
        }

        public void Serialize(ServerMessage message, bool friendBar)
        {
            if (friendBar)
            {
                message.AppendUInt(this.ID);
                message.AppendStringWithBreak(this.Username);
                message.AppendInt32(0); //unknown
                message.AppendBoolean(this.IsOnline); //online
                message.AppendBoolean(this.InRoom); //in room
                message.AppendStringWithBreak(this.Look); //look
                message.AppendInt32(0); //category
                message.AppendStringWithBreak(this.Motto);
                message.AppendStringWithBreak(TimeUtilies.UnixTimestampToDateTime(this.LastLogin).ToString());
                message.AppendStringWithBreak(""); //real name
                message.AppendBoolean(false);
            }
            else
            {
                message.AppendUInt(this.ID);
                message.AppendStringWithBreak(this.Username);
                message.AppendStringWithBreak(this.Motto);
                message.AppendBoolean(this.IsOnline); //online
                message.AppendBoolean(this.InRoom); //in room
                message.AppendStringWithBreak(""); //nothing
                message.AppendInt32(0); //category
                message.AppendStringWithBreak(this.Look);
                message.AppendStringWithBreak(TimeUtilies.UnixTimestampToDateTime(this.LastLogin).ToString());
                message.AppendStringWithBreak(""); //real name
            }
        }
    }
}
