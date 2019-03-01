using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredJoinTeam : RoomItemWiredAction
    {
        public GameTeam Team;

        public RoomItemWiredJoinTeam(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Team = GameTeam.Red;
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
                message.AppendString(""); //data
                message.AppendInt32(1); //extra data count
                message.AppendInt32((int)this.Team); //team
                message.AppendInt32(0); //idk

                message.AppendInt32(9); //type
                message.AppendInt32(0); //delay, not work with this wired
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override void LoadItemData(string data)
        {
            this.Team = (GameTeam)int.Parse(data);
        }

        public override string GetItemData()
        {
            return ((int)this.Team).ToString();
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
            if (triggerer != null)
            {
                this.Room.RoomGameManager.JoinTeam(triggerer, this.Team, GameType.Freeze);
            }
            else
            {
                foreach(RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
                {
                    this.Room.RoomGameManager.JoinTeam(user, this.Team, GameType.Freeze);
                }
            }
        }
    }
}