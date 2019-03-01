using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
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
        public readonly int Category;
        public readonly string Username;
        public readonly string Look;
        public readonly string Motto;
        public readonly double LastLogin;
        public MessengerFriendRelation Relation;
        public bool NeedUpdate = false;

        public bool IsOnline
        {
            get
            {
                if (this.ID != 0)
                {
                    GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(this.ID);
                    return gameClient != null && !gameClient.Disconnected && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null && !gameClient.GetHabbo().GetUserSettings().HideOnline;
                }
                else
                {
                    return true;
                }
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

        public MessengerFriend(uint id, int category, string look, string motto, double lastLogin, MessengerFriendRelation relation)
        {
            this.ID = id;
            if (id > 0)
            {
                this.Username = Skylight.GetGame().GetGameClientManager().GetUsernameByID(id);
            }
            else if (id == 0)
            {
                this.Username = "Staff Chat";
            }
            this.Category = category;
            this.Look = look;
            this.Motto = motto;
            this.LastLogin = lastLogin;
            this.Relation = relation;
        }

        public void Serialize(ServerMessage message, bool friendBar)
        {
            if (friendBar)
            {
                message.AppendUInt(this.ID);
                message.AppendString(this.Username);
                message.AppendInt32(0); //gender, ye, its int
                message.AppendBoolean(this.IsOnline); //online
                message.AppendBoolean(this.InRoom); //in room
                message.AppendString(this.Look); //look
                message.AppendInt32(this.Category); //category
                message.AppendString(this.Motto);
                if (message.GetRevision() < Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendString(TimeUtilies.UnixTimestampToDateTime(this.LastLogin).ToString());
                }

                if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                {
                    message.AppendString(""); //real name
                    message.AppendString(""); //un used
                    if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                    {
                        message.AppendBoolean(false); //offline messages enabled
                        message.AppendBoolean(true); //idk
                        message.AppendBoolean(false); //pocked habbo
                        if (message.GetRevision() >= Revision.PRODUCTION_201601012205_226667486)
                        {
                            message.AppendShort(0); //Relation
                        }
                    }
                }
            }
            else
            {
                message.AppendUInt(this.ID);
                message.AppendString(this.Username);
                message.AppendString(this.Motto);
                message.AppendBoolean(this.IsOnline); //online
                message.AppendBoolean(this.InRoom); //in room
                message.AppendString(""); //nothing
                message.AppendInt32(this.Category); //category
                message.AppendString(this.Look);
                message.AppendString(TimeUtilies.UnixTimestampToDateTime(this.LastLogin).ToString());

                if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                {
                    message.AppendString(""); //real name
                }
            }
        }
    }
}
