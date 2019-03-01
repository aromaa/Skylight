using SkylightEmulator.HabboHotel.Rooms.Games;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomGameManager
    {
        public readonly Room Room;
        public RoomFootballManager RoomFootballManager;
        public RoomBattleBanzaiManager RoomBattleBanzaiManager;
        public RoomWobbleSquabbleManager RoomWobbleSquabbleManager;
        public Dictionary<uint, RoomUnitUser> UsersInGame;

        //battle banzai and freeze uses same scoreboards
        public Dictionary<uint, RoomItem> GreenScoreboards;
        public Dictionary<uint, RoomItem> BlueScoreboards;
        public Dictionary<uint, RoomItem> YellowScoreboards;
        public Dictionary<uint, RoomItem> RedScoreboards;
        public bool GameStarted;

        //ice tag
        public Dictionary<uint, RoomUnitUser> TagPlayers;

        //old school love
        public Dictionary<uint, RoomGameboard> Gameboards;

        public RoomGameManager(Room room)
        {
            this.Room = room;
            this.RoomFootballManager = new RoomFootballManager(this.Room);
            this.RoomBattleBanzaiManager = new RoomBattleBanzaiManager(this.Room);
            this.RoomWobbleSquabbleManager = new RoomWobbleSquabbleManager(this.Room);
            this.UsersInGame = new Dictionary<uint, RoomUnitUser>();
            this.Gameboards = new Dictionary<uint, RoomGameboard>();

            this.GreenScoreboards = new Dictionary<uint, RoomItem>();
            this.BlueScoreboards = new Dictionary<uint, RoomItem>();
            this.YellowScoreboards = new Dictionary<uint, RoomItem>();
            this.RedScoreboards = new Dictionary<uint, RoomItem>();
            
            this.TagPlayers = new Dictionary<uint, RoomUnitUser>();

            if (this.Room.RoomData.IsPublicRoom)
            {
                foreach(RoomModelTrigger trigger in this.Room.RoomGamemapManager.Model.Triggers)
                {
                    if (trigger != null)
                    {
                        if (trigger.Type == "open_gameboard")
                        {
                            if (!this.Gameboards.ContainsKey(uint.Parse(trigger.Data[1])))
                            {
                                this.Gameboards.Add(uint.Parse(trigger.Data[1]), new RoomGameboard(trigger.Data[0]));
                            }
                        }
                    }
                }
            }
        }

        public void OnCycle()
        {
            this.RoomFootballManager.OnCycle();
            this.RoomBattleBanzaiManager.OnCycle();
            this.Room.RoomFreezeManager.OnCycle();
            this.RoomWobbleSquabbleManager.OnCycle();

            foreach(RoomItemTagPole pole in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemTagPole)))
            {
                if (pole.Tagged == null)
                {
                    pole.Tagged = this.TagPlayers.Values.OrderBy(r => RandomUtilies.GetRandom(int.MinValue, int.MaxValue - 1)).FirstOrDefault(u => u.IceSkateStatus == IceSkateStatus.Playing);
                    if (pole.Tagged != null)
                    {
                        pole.Tagged.IceSkateStatus = IceSkateStatus.Tagged;
                        this.GiveEffect(pole.Tagged);
                    }
                }
            }
        }

        public void JoinTeam(RoomUnitUser user, GameTeam team, GameType type)
        {
            if (!this.UsersInGame.ContainsKey(user.Session.GetHabbo().ID))
            {
                this.UsersInGame.Add(user.Session.GetHabbo().ID, user);
            }

            user.GameTeam = team;
            user.GameType = type;
            this.GiveEffect(user);
        }

        public void JoinTag(RoomUnitUser user)
        {
            if (!this.TagPlayers.ContainsKey(user.Session.GetHabbo().ID))
            {
                this.TagPlayers.Add(user.Session.GetHabbo().ID, user);
                user.IceSkateStatus = IceSkateStatus.Playing;

                foreach (RoomItemTagPole pole in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemTagPole)))
                {
                    if (pole.Tagged == null)
                    {
                        pole.Tagged = user;
                        user.IceSkateStatus = IceSkateStatus.Tagged;
                    }
                }
            }

            this.GiveEffect(user);
        }

        public void LeaveTag(RoomUnitUser user)
        {
            this.TagPlayers.Remove(user.Session.GetHabbo().ID);
            user.IceSkateStatus = IceSkateStatus.None;

            foreach (RoomItemTagPole pole in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemTagPole)))
            {
                if (pole.Tagged == user)
                {
                    pole.Tagged = this.TagPlayers.Values.OrderBy(r => RandomUtilies.GetRandom(int.MinValue, int.MaxValue - 1)).FirstOrDefault(u => u.IceSkateStatus == IceSkateStatus.Playing);
                    if (pole.Tagged != null)
                    {
                        pole.Tagged.IceSkateStatus = IceSkateStatus.Tagged;
                        this.GiveEffect(pole.Tagged);
                    }
                }
            }

            this.GiveEffect(user);
        }

        public void LeaveTeam(RoomUnitUser user)
        {
            this.UsersInGame.Remove(user.Session.GetHabbo().ID);

            user.GameTeam = GameTeam.None;
            user.GameType = GameType.None;
            this.GiveEffect(user);
        }

        public void GiveEffect(RoomUnitUser user)
        {
            if (user.Rollerskate)
            {
                if (user.Session.GetHabbo().Gender == "M")
                {
                    user.ApplyEffect(55);
                }
                else
                {
                    user.ApplyEffect(56);
                }
            }
            else if (user.IceSkateStatus == IceSkateStatus.Playing)
            {
                if (user.Session.GetHabbo().Gender == "M")
                {
                    user.ApplyEffect(38);
                }
                else
                {
                    user.ApplyEffect(39);
                }
            }
            else if (user.IceSkateStatus == IceSkateStatus.Tagged)
            {
                if (user.Session.GetHabbo().Gender == "M")
                {
                    user.ApplyEffect(45);
                }
                else
                {
                    user.ApplyEffect(46);
                }
            }
            else if (user.GameType == GameType.None)
            {
                user.ApplyEffect(-1);
            }
            else if (user.GameType == GameType.Freeze)
            {
                user.ApplyEffect(39 + (int)user.GameTeam);
            }
            else if (user.GameType == GameType.BattleBanzai)
            {
                user.ApplyEffect(32 + (int)user.GameTeam);
            }
        }

        public void AddItem(RoomItem item)
        {
            this.Room.RoomFreezeManager.AddItem(item);

            if (item.GetBaseItem().ItemName == "bb_score_g")
            {
                this.GreenScoreboards.Add(item.ID, item);
            }
            else if (item.GetBaseItem().ItemName == "bb_score_b")
            {
                this.BlueScoreboards.Add(item.ID, item);
            }
            else if (item.GetBaseItem().ItemName == "bb_score_y")
            {
                this.YellowScoreboards.Add(item.ID, item);
            }
            else if (item.GetBaseItem().ItemName == "bb_score_r")
            {
                this.RedScoreboards.Add(item.ID, item);
            }
        }

        public void RemoveItem(RoomItem item)
        {
            this.Room.RoomFreezeManager.RemoveItem(item);

            if (item.GetBaseItem().ItemName == "bb_score_g")
            {
                this.GreenScoreboards.Remove(item.ID);
            }
            else if (item.GetBaseItem().ItemName == "bb_score_b")
            {
                this.BlueScoreboards.Remove(item.ID);
            }
            else if (item.GetBaseItem().ItemName == "bb_score_y")
            {
                this.YellowScoreboards.Remove(item.ID);
            }
            else if (item.GetBaseItem().ItemName == "bb_score_r")
            {
                this.RedScoreboards.Remove(item.ID);
            }
        }

        internal void ToggleGameStatus()
        {
            if (this.GameStarted)
            {
                this.StopGame();
            }
            else
            {
                this.StartGame();
            }
        }

        public void StartGame()
        {
            if (!this.GameStarted)
            {
                this.GameStarted = true;

                this.Room.RoomFreezeManager.StartGame();
                this.RoomBattleBanzaiManager.StartGame();
                this.Room.RoomWiredManager.GameStart();
            }
        }

        public void StopGame()
        {
            if (this.GameStarted)
            {
                this.GameStarted = false;

                this.Room.RoomFreezeManager.StopGame();
                this.RoomBattleBanzaiManager.StopGame();
                this.Room.RoomWiredManager.GameEnd();
            }
        }
    }
}
