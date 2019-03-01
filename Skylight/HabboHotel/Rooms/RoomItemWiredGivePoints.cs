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
    class RoomItemWiredGivePoints : RoomItemWiredAction
    {
        public int Points;
        public int PointsAmount;
        public int PointsAmountUsed;

        public RoomItemWiredGivePoints(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
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
                message.AppendInt32(2); //extra data count
                message.AppendInt32(this.Points);
                message.AppendInt32(this.PointsAmount);
                message.AppendInt32(0); //delay, not work with this wired

                message.AppendInt32(6); //type
                message.AppendInt32(this.Delay); //delay
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return this.Points.ToString() + (char) 9 + this.PointsAmount + (char) 9 + this.Delay;
        }

        public override void LoadItemData(string data)
        {
            string[] splitData = data.Split((char)9);
            this.Points = int.Parse(splitData[0]);
            this.PointsAmount = int.Parse(splitData[1]);
            this.Delay = int.Parse(splitData[2]);
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
            if (this.PointsAmountUsed < this.PointsAmount)
            {
                this.PointsAmountUsed++;

                if (triggerer != null)
                {
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(triggerer, triggerer.GameTeam, this.Points);

                    this.Room.RoomGameManager.RoomBattleBanzaiManager.UpdateScore(triggerer.GameTeam);
                }
                else
                {
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(triggerer, GameTeam.Blue, this.Points);
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(triggerer, GameTeam.Green, this.Points);
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(triggerer, GameTeam.Yellow, this.Points);
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.AddScore(triggerer, GameTeam.Red, this.Points);

                    this.Room.RoomGameManager.RoomBattleBanzaiManager.UpdateScore(GameTeam.Blue);
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.UpdateScore(GameTeam.Green);
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.UpdateScore(GameTeam.Yellow);
                    this.Room.RoomGameManager.RoomBattleBanzaiManager.UpdateScore(GameTeam.Red);
                }
            }
        }
    }
}
