using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    class RoomItemWiredActionTrigger : RoomItemWiredAction
    {
        public HashSet<uint> TriggeredUsers;

        public RoomItemWiredActionTrigger(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.TriggeredUsers = new HashSet<uint>();
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                string motd = "- - - Triggered users - - -\r\n";
                foreach(uint userId in this.TriggeredUsers)
                {
                    motd += Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId) + "\r\n";
                }
                session.SendNotif(motd, 2);
            }
        }

        public override string GetItemData()
        {
            return string.Join(",", this.TriggeredUsers);
        }

        public override void LoadItemData(string data)
        {
            foreach (string sUserId in data.Split(','))
            {
                this.TriggeredUsers.Add(uint.Parse(sUserId));
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
                if (triggerer.IsRealUser)
                {
                    if (this.TriggeredUsers.Add(triggerer.UserID))
                    {
                        this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);
                    }
                }
            }
            else
            {
                foreach(RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
                {
                    if (this.TriggeredUsers.Add(user.UserID))
                    {
                        this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);
                    }
                }
            }
        }
    }
}
