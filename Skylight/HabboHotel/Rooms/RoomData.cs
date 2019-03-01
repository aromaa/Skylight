using Newtonsoft.Json;
using SkylightEmulator.Core;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomData
    {
        public uint ID;
        public string Name;
        public string Description;
        public uint OwnerID;
        public string Type;
        public string Model;
        public RoomStateType State;
        public int Category;
        public int UsersNow;
        public int UsersMax;
        public string PublicCCTs;
        public int Score;
        public List<string> Tags;
        public RoomIcon RoomIcon;
        public string Password;
        public string Wallpaper;
        public string Floor;
        public string Landscape;
        public bool AllowPets;
        public bool AllowPetsEat;
        public bool AllowWalkthrough;
        public bool Hidewalls;
        public int Wallthick;
        public int Floorthick;
        public RoomExtraData ExtraData;
        public RoomAllowTradeType AllowTrade;
        public RoomWhoCanType MuteOption;
        public RoomWhoCanType KickOption;
        public RoomWhoCanType BanOption;
        public RoomChatModeType ChatMode;
        public RoomChatWeightType ChatWeight;
        public RoomChatSpeedType ChatSpeed;
        public int ChatDistance;
        public RoomChatProtectionType ChatProtection;

        public bool IsStaffPick
        {
            get
            {
                return Skylight.GetGame().GetNavigatorManager().GetPublicItem(this.ID, ServerConfiguration.StaffPicksCategoryId) != null;
            }
        }

        public RoomData(DataRow dataRow)
        {
            if (dataRow != null)
            {
                this.ID = (uint)dataRow["id"];
                this.Name = (string)dataRow["name"];
                this.Description = (string)dataRow["description"];
                this.OwnerID = (uint)dataRow["ownerid"];
                this.Type = (string)dataRow["type"];
                this.Model = (string)dataRow["model"];
                this.State = (string)dataRow["state"] == "password" ? RoomStateType.PASSWORD : (string)dataRow["state"] == "locked" ? RoomStateType.LOCKED : RoomStateType.OPEN;
                this.Category = (int)dataRow["category"];
                this.UsersNow = (int)dataRow["users_now"]; //maybe we need this sometimes :3
                this.UsersMax = (int)dataRow["users_max"];
                this.PublicCCTs = (string)dataRow["public_ccts"];
                this.Score = (int)dataRow["score"];
                string tags = (string)dataRow["tags"];
                if (!string.IsNullOrEmpty(tags))
                {
                    this.Tags = tags.Split(',').ToList();
                }
                else
                {
                    this.Tags = new List<string>();
                }
                this.RoomIcon = new RoomIcon((int)dataRow["icon_bg"], (int)dataRow["icon_fg"], (string)dataRow["icon_items"]);
                this.Password = (string)dataRow["password"];
                this.Wallpaper = (string)dataRow["wallpaper"];
                this.Floor = (string)dataRow["floor"];
                this.Landscape = (string)dataRow["landscape"];
                this.AllowPets = TextUtilies.StringToBool((string)dataRow["allow_pets"]);
                this.AllowPetsEat = TextUtilies.StringToBool((string)dataRow["allow_pets_eat"]);
                this.AllowWalkthrough = TextUtilies.StringToBool((string)dataRow["allow_walkthrough"]);
                this.Hidewalls = TextUtilies.StringToBool((string)dataRow["hidewalls"]);
                this.Wallthick = (int)dataRow["wallthick"];
                this.Floorthick = (int)dataRow["floorthick"];
                this.AllowTrade = (RoomAllowTradeType)int.Parse((string)dataRow["trade"]);
                this.MuteOption = (RoomWhoCanType)int.Parse((string)dataRow["mute_option"]);
                this.KickOption = (RoomWhoCanType)int.Parse((string)dataRow["kick_option"]);
                this.BanOption = (RoomWhoCanType)int.Parse((string)dataRow["ban_option"]);
                this.ChatMode = (RoomChatModeType)int.Parse((string)dataRow["chat_mode"]);
                this.ChatWeight = (RoomChatWeightType)int.Parse((string)dataRow["chat_weight"]);
                this.ChatSpeed = (RoomChatSpeedType)int.Parse((string)dataRow["chat_speed"]);
                this.ChatProtection = (RoomChatProtectionType)int.Parse((string)dataRow["chat_protection"]);

                string data = (string)dataRow["data"];
                if (!string.IsNullOrEmpty(data))
                {
                    this.ExtraData = JsonConvert.DeserializeObject<RoomExtraData>(data);
                }
                else
                {
                    this.ExtraData = new RoomExtraData();
                }
            }
            else
            {
                this.NullValues();
            }
        }

        public void NullValues()
        {
            this.ID = 0;
            this.Name = "";
            this.Description = "";
            this.OwnerID = 0;
            this.Type = "private";
            this.Model = "";
            this.State = RoomStateType.OPEN;
            this.Category = 0;
            this.UsersNow = 0;
            this.UsersMax = 25;
            this.PublicCCTs = "";
            this.Score = 0;
            this.Tags = new List<string>();
            this.RoomIcon = new RoomIcon(1, 0, "");
            this.Password = "";
            this.Wallpaper = "0.0";
            this.Floor = "0.0";
            this.Landscape = "0.0";
            this.AllowPets = true;
            this.AllowPetsEat = false;
            this.AllowWalkthrough = false;
            this.Hidewalls = false;
            this.Wallthick = 0;
            this.Floorthick = 0;
            this.ExtraData = new RoomExtraData();
        }

        public void Serialize(ServerMessage message, bool asEvent)
        {
            if (asEvent) asEvent = this.GetRoom().RoomEvent != null;

            if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                message.AppendUInt(this.ID);
                if (message.GetRevision() < Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendBoolean(asEvent); //event
                }
                message.AppendString(asEvent ? this.GetRoom().RoomEvent.Name : this.Name); //room name / event name
                if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                {
                    if (message.GetRevision() <= Revision.RELEASE63_201211141113_913728051)
                    {
                        message.AppendBoolean(this.Type == "private");
                    }
                    message.AppendUInt(this.OwnerID);
                }
                message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.OwnerID));
                message.AppendInt32((int)this.State); //state
                message.AppendInt32(this.UsersNow);
                message.AppendInt32(this.UsersMax); //users max
                message.AppendString(asEvent ? this.GetRoom().RoomEvent.Description : this.Description); //event desc / room desc
                message.AppendInt32(0); //unknown
                if (message.GetRevision() < Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendBoolean(true); //can trade
                }
                else
                {
                    message.AppendInt32(0); //can trade
                }
                message.AppendInt32(this.Score); //score
                if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051 && message.GetRevision() < Revision.PRODUCTION_201601012205_226667486)
                {
                    message.AppendInt32(0); //most points ranking
                }
                message.AppendInt32(asEvent ? this.GetRoom().RoomEvent.Category : this.Category); //category / event category
                if (message.GetRevision() < Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendString(asEvent ? this.GetRoom().RoomEvent.StartTime.ToShortTimeString() : "0"); //event start time
                }
                else
                {
                    if (message.GetRevision() < Revision.PRODUCTION_201601012205_226667486)
                    {
                        message.AppendInt32(0); //group id
                        message.AppendString(""); //group name
                        message.AppendString(""); //group badge
                        message.AppendString("");
                    }
                }

                if (asEvent)
                {
                    message.AppendInt32(this.GetRoom().RoomEvent.Tags.Count); //event tags count
                    foreach (string tag in this.GetRoom().RoomEvent.Tags)
                    {
                        message.AppendString(tag);
                    }
                }
                else
                {
                    message.AppendInt32(this.Tags.Count); //tags count
                    foreach (string tag in this.Tags)
                    {
                        message.AppendString(tag);
                    }
                }

                if (message.GetRevision() < Revision.PRODUCTION_201601012205_226667486)
                {
                    this.RoomIcon.Serialize(message);
                }
                else
                {
                    int base_ = 8 | 16;

                    /*if(hasGroup)
                    {
                        base_ = base_ | 2;
                    }*/

                    if(!this.IsPublicRoom)
                    {
                        base_ = base_ | 8;
                    }

                    if(asEvent)
                    {
                        base_ = base_ | 4;
                    }

                    message.AppendInt32(base_);

                    /*if (hasGroup)
                    {
                        message.AppendInt32(0); //id
                        message.AppendString("Name");
                        message.AppendString("Badge");
                    }*/

                    if (asEvent)
                    {
                        message.AppendString(this.GetRoom().RoomEvent.Name);
                        message.AppendString(this.GetRoom().RoomEvent.Description);
                        message.AppendInt32(30); //Time left in mins
                    }
                }

                if (message.GetRevision() < Revision.PRODUCTION_201601012205_226667486)
                {
                    message.AppendBoolean(this.AllowPets); //allow pets
                    message.AppendBoolean(this.AllowPetsEat); //allow pets eating
                    if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                    {
                        message.AppendString(""); //room event
                        message.AppendString(""); //room event
                        message.AppendInt32(0); //room event
                    }
                }
            }
            else
            {
                message.AppendInt32(0); //everyone has rights
                message.AppendInt32((int)this.State);
                message.AppendUInt(this.ID);
                message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(this.OwnerID));
                message.AppendString(this.Model);
                message.AppendString(this.Name);
                message.AppendString(this.Description);
                message.AppendInt32(0); //show name
                message.AppendBoolean(true); //can trade
                message.AppendInt32(this.UsersNow);
                message.AppendInt32(this.UsersMax);
            }
        }

        public void SerializeRoomEntry(ServerMessage message, bool entry, bool forward)
        {
            message.AppendBoolean(entry);
            this.Serialize(message, false);
            message.AppendBoolean(forward);
            message.AppendBoolean(this.IsStaffPick);
            message.AppendBoolean(this.IsPublicRoom);
            if (this.GetRoom() != null)
            {
                message.AppendBoolean(this.GetRoom().RoomMute);
            }
            else
            {
                message.AppendBoolean(false);
            }

            message.AppendInt32((int)this.MuteOption); //Mute option
            message.AppendInt32((int)this.KickOption); //Kick option
            message.AppendInt32((int)this.BanOption); //Ban option

            message.AppendBoolean(false); //Mute all button

            message.AppendInt32((int)this.ChatMode); //Chat mode
            message.AppendInt32((int)this.ChatWeight); //Chat weight
            message.AppendInt32((int)this.ChatSpeed); //Chat speed
            message.AppendInt32(this.ChatDistance); //Chat distance
            message.AppendInt32((int)this.ChatProtection); //Chat protection
        }

        public Room GetRoom()
        {
            return Skylight.GetGame().GetRoomManager().TryGetRoom(this.ID);
        }

        public bool IsPublicRoom
        {
            get
            {
                return this.Type == "public";
            }
        }
    }
}
