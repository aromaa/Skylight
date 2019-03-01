using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredActionSkylight : RoomItemWiredAction
    {
        public string Message;
        public RoomItemWiredActionSkylight(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Message = "";
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredAction);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(0); //furni limit
                message.AppendInt32(0); //furni count
                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(this.Message); //data
                message.AppendInt32(0); //extra data count
                message.AppendInt32(0); //idk

                message.AppendInt32(7); //type
                message.AppendInt32(0); //delay, not work with this wired
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return this.Message;
        }

        public override void LoadItemData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                string[] data_ = data.Split(new char[] { ':' }, 2);
                if (this.IsValidFormat(data_[0], data_.Length > 1 ? data_[1] : "", out string correctFormat))
                {
                    this.Message = data;
                }
            }
        }

        public override void OnLoad()
        {
            this.ExtraData = "0";
        }

        public override void OnPickup(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnPlace(GameClient session)
        {
            this.ExtraData = "0";
        }

        public override void OnMove(GameClient session)
        {
            if (session == null || !session.GetHabbo().HasPermission("wired_action_" + this.Message.Split(':')[0]))
            {
                this.Message = "";
            }
        }

        public override void OnCycle()
        {
            if (this.UpdateNeeded)
            {
                this.UpdateTimer--;
                if (this.UpdateTimer <= 0)
                {
                    this.UpdateNeeded = false;

                    this.ExtraData = "0";
                    this.UpdateState(false, true);
                }
            }
        }

        public override void DoWiredAction(RoomUnitUser triggerer, HashSet<uint> used)
        {
            if (!string.IsNullOrEmpty(this.Message))
            {
                string[] data = this.Message.Split(new char[] { ':' }, 2);
                if (triggerer != null)
                {
                    this.DoWiredAction(triggerer, data[0], data.Length > 1 ? data[1] : "");
                }
                else
                {
                    foreach (RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
                    {
                        this.DoWiredAction(user, data[0], data.Length > 1 ? data[1] : "");
                    }
                }
            }
        }

        private void DoWiredAction(RoomUnitUser target, string command, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                data = data.Replace("%username%", target.Session.GetHabbo().Username);
                data = data.Replace("%userid%", target.Session.GetHabbo().ID.ToString());
            }

            switch(command)
            {
                case "sql":
                    {
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.ExecuteQuery(data);
                        }
                    }
                    break;
                case "badge":
                    {
                        target.Session.GetHabbo().GetBadgeManager().AddBadge(data, 0, true);
                    }
                    break;
                case "award":
                    {
                        Skylight.GetGame().GetAchievementManager().AddAchievement(target.Session, data);
                    }
                    break;
                case "dance":
                    {
                        target.SetDance(int.Parse(data));
                    }
                    break;
                case "send":
                    {
                        Room room = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoom(uint.Parse(data));
                        if (room != null)
                        {
                            ServerMessage followFriend = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                            followFriend.Init(r63aOutgoing.RoomForward);
                            followFriend.AppendBoolean(room.RoomData.IsPublicRoom);
                            followFriend.AppendUInt(room.ID);
                            target.Session.SendMessage(followFriend);
                        }
                        else
                        {
                            this.Room.RoomUserManager.KickUser(target.Session, false);
                        }
                    }
                    break;
                case "credits":
                    {
                        target.Session.GetHabbo().Credits += int.Parse(data);
                        target.Session.GetHabbo().UpdateCredits(true);
                    }
                    break;
                case "pixels":
                    {
                        target.Session.GetHabbo().AddActivityPoints(0, int.Parse(data));
                        target.Session.GetHabbo().UpdateActivityPoints(0, true);
                    }
                    break;
                case "points":
                    {
                        string[] data_ = data.Split(',');
                        target.Session.GetHabbo().AddActivityPoints(int.Parse(data_[0]), int.Parse(data_[1]));
                        target.Session.GetHabbo().UpdateActivityPoints(int.Parse(data_[0]), true);
                    }
                    break;
                case "shells":
                    {
                        target.Session.GetHabbo().AddActivityPoints(4, int.Parse(data));
                        target.Session.GetHabbo().UpdateActivityPoints(4, true);
                    }
                    break;
                case "snowflakes":
                    {
                        target.Session.GetHabbo().AddActivityPoints(1, int.Parse(data));
                        target.Session.GetHabbo().UpdateActivityPoints(1, true);
                    }
                    break;
                case "hearts":
                    {
                        target.Session.GetHabbo().AddActivityPoints(2, int.Parse(data));
                        target.Session.GetHabbo().UpdateActivityPoints(2, true);
                    }
                    break;
                case "giftpoints":
                    {
                        target.Session.GetHabbo().AddActivityPoints(3, int.Parse(data));
                        target.Session.GetHabbo().UpdateActivityPoints(3, true);
                    }
                    break;
                case "rank":
                    {
                        //using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        //{
                        //    dbClient.AddParamWithValue("rank", int.Parse(data));
                        //    dbClient.AddParamWithValue("userId", Session.GetHabbo().ID);
                        //    dbClient.ExecuteQuery("UPDATE users SET rank = @rank WHERE id = @userId LIMIT 1");
                        //}
                        //Session.Stop("Super wired 'rank' used");
                    }
                    break;
                case "respect":
                    {
                        target.Session.GetHabbo().GetUserStats().RespectReceived++;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("receiverId", target.Session.GetHabbo().ID);
                            dbClient.AddParamWithValue("receiverRespects", target.Session.GetHabbo().GetUserStats().RespectReceived);
                            dbClient.ExecuteQuery("UPDATE user_stats SET respect_received = @receiverRespects WHERE user_id = @receiverId LIMIT 1");
                        }
                        
                        this.Room.SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.GiveRespect, new ValueHolder("UserID", target.Session.GetHabbo().ID, "Total", target.Session.GetHabbo().GetUserStats().RespectReceived)));

                        target.Session.GetHabbo().GetUserAchievements().CheckAchievement("RespectReceived");
                    }
                    break;
                case "handitem":
                    {
                        target.SetHanditem(int.Parse(data));
                    }
                    break;
                case "alert":
                    {
                        target.Session.SendNotif(data);
                    }
                    break;
                case "motd":
                    {
                        target.Session.SendNotif(data, 2);
                    }
                    break;
                default:
                    {                 
                    }
                    break;
            }
        }

        public bool IsValidFormat(string command, string data, out string correctFormat)
        {
            switch (command)
            {
                case "sql":
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            correctFormat = "sql:<sql>";
                            return false;
                        }
                    }
                    break;
                case "badge":
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            correctFormat = "badge:<badge code>";
                            return false;
                        }
                    }
                    break;
                case "award":
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            correctFormat = "award:<achievement name>" + "\n" + "award:<achievement name> <level>";
                            return false;
                        }
                        else
                        {
                            string[] data_ = data.Split(',');
                            if (data_.Length == 2)
                            {
                                if (!int.TryParse(data_[1], out int level))
                                {
                                    correctFormat = "award:<achievement name> <level>";
                                    return false;
                                }
                            }
                            else if (data_.Length >= 2)
                            {
                                correctFormat = "award:<achievement name>" + "\n" + "award:<achievement name> <level>";
                                return false;
                            }
                        }
                    }
                    break;
                case "dance":
                    {
                        if (!int.TryParse(data, out int danceId))
                        {
                            correctFormat = "dance:<dance id>";
                            return false;
                        }
                    }
                    break;
                case "send":
                    {
                        if (!uint.TryParse(data, out uint roomId))
                        {
                            correctFormat = "send:<room id>";
                            return false;
                        }
                    }
                    break;
                case "credits":
                    {
                        if (!int.TryParse(data, out int credits))
                        {
                            correctFormat = "credits:<amount>";
                            return false;
                        }
                    }
                    break;
                case "pixels":
                    {
                        if (!int.TryParse(data, out int pixels))
                        {
                            correctFormat = "pixels:<amount>";
                            return false;
                        }
                    }
                    break;
                case "points":
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            correctFormat = "points:<id>,<amount>";
                            return false;
                        }
                        else
                        {
                            string[] data_ = data.Split(',');
                            if (data_.Length != 2)
                            {
                                correctFormat = "points:<id>,<amount>";
                                return false;
                            }
                            else
                            {
                                if (!int.TryParse(data_[0], out int id) || !int.TryParse(data_[1], out int amount))
                                {
                                    correctFormat = "points:<id>,<amount>";
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                case "shells":
                    {
                        if (!int.TryParse(data, out int shells))
                        {
                            correctFormat = "shells:<amount>";
                            return false;
                        }
                    }
                    break;
                case "snowflakes":
                    {
                        if (!int.TryParse(data, out int snowflakes))
                        {
                            correctFormat = "snowflakes:<mount>";
                            return false;
                        }
                    }
                    break;
                case "hearts":
                    {
                        if (!int.TryParse(data, out int hearts))
                        {
                            correctFormat = "hearts:<amount>";
                            return false;
                        }
                    }
                    break;
                case "giftpoints":
                    {
                        if (!int.TryParse(data, out int giftPoints))
                        {
                            correctFormat = "giftpoints:<amount>";
                            return false;
                        }
                    }
                    break;
                case "rank":
                    {
                        //using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        //{
                        //    dbClient.AddParamWithValue("rank", int.Parse(data));
                        //    dbClient.AddParamWithValue("userId", Session.GetHabbo().ID);
                        //    dbClient.ExecuteQuery("UPDATE users SET rank = @rank WHERE id = @userId LIMIT 1");
                        //}
                        //Session.Stop("Super wired 'rank' used");
                        correctFormat = "rank:<id>";
                        return false;
                    }
                case "respect":
                    {
                        correctFormat = "respect:";
                        return true;
                    }
                case "handitem":
                    {
                        if (!int.TryParse(data, out int handitem))
                        {
                            correctFormat = "handitem:<id>";
                            return false;
                        }
                    }
                    break;
                case "alert":
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            correctFormat = "alert:<message>";
                            return false;
                        }
                    }
                    break;
                case "motd":
                    {
                        if (string.IsNullOrEmpty(data))
                        {
                            correctFormat = "motd:<message>";
                            return false;
                        }
                    }
                    break;
                default:
                    {
                        correctFormat = "Unknown command";
                        return false;
                    }
            }

            correctFormat = "";
            return true;
        }
    }
}
