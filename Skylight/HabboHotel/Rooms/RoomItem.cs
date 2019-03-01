using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItem
    {
        public uint ID;
        public uint RoomID;
        public uint UserID;
        public Item BaseItem;
        public string ExtraData;
        public int X;
        public int Y;
        public double Z;
        public int Rot;
        public WallCoordinate WallCoordinate;
        public Room Room;
        public HashSet<AffectedTile> AffectedTiles;
        public MoodlightData MoodlightData;

        public bool UpdateNeeded;
        public int UpdateTimer;
        public UpdateStatus RoomUpdateStatus;

        public RoomItem(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
        {
            this.ID = id;
            this.RoomID = roomId;
            this.UserID = userId;
            this.BaseItem = baseItem;
            this.ExtraData = extraData;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Rot = rot;
            this.WallCoordinate = wallCoordinate;
            this.Room = room;
            if (this.BaseItem.InteractionType.ToLower() == "dimmer")
            {
                this.MoodlightData = new MoodlightData(this.ID);
            }
            else
            {
                this.MoodlightData = null;
            }

            this.AffectedTiles = new HashSet<AffectedTile>();
            this.CalcAffectedTiles();
        }

        public void CalcAffectedTiles()
        {
            this.AffectedTiles.Clear();
            this.AffectedTiles.UnionWith(ItemUtilies.AffectedTiles(this.BaseItem.Lenght, this.BaseItem.Width, this.X, this.Y, this.Rot).Values);
        }

        public static RoomItem GetRoomItem(uint id, uint roomId, uint userId, uint baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
        {
            Item baseItem_ = Skylight.GetGame().GetItemManager().TryGetItem(baseItem);

            switch (baseItem_.InteractionType.ToLower())
            {
                case "teleport":
                    {
                        return new RoomItemTeleport(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bottle":
                    {
                        return new RoomItemBottle(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "dice":
                    {
                        return new RoomItemDice(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "habbowheel":
                    {
                        return new RoomItemHabbowheel(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "loveshuffler":
                    {
                        return new RoomItemLoveshuffler(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "vendingmachine":
                    {
                        return new RoomItemVendingmachine(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_enterroom":
                    {
                        return new RoomItemWiredEnterRoom(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_saymsg":
                    {
                        return new RoomItemWiredSayMessage(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_time_more_than":
                    {
                        return new RoomItemWiredTimeMoreThan(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_xtra_random":
                    {
                        return new RoomItemWiredRandom(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_xtra_unseen":
                    {
                        return new RoomItemWiredUnseen(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_gameend":
                    {
                        return new RoomItemWiredGameEnd(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_attime":
                    {
                        return new RoomItemWiredAtTime(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_offfurni":
                    {
                        return new RoomItemWiredOffFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_onsay":
                    {
                        return new RoomItemWiredOnSay(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_atscore":
                    {
                        return new RoomItemWiredAtScore(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_timer":
                    {
                        return new RoomItemWiredTimer(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_furnistate":
                    {
                        return new RoomItemWiredFurniState(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_gamestart":
                    {
                        return new RoomItemWiredGameStart(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_onfurni":
                    {
                        return new RoomItemWiredOnFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_moverotate":
                    {
                        return new RoomItemWiredMoveRotate(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_givepoints":
                    {
                        return new RoomItemWiredGivePoints(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_reset_timers":
                    {
                        return new RoomItemWiredResetTimers(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_togglefurni":
                    {
                        return new RoomItemWiredToggleFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_moveuser":
                    {
                        return new RoomItemWiredMoveUser(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_matchfurni":
                    {
                        return new RoomItemWiredMatchFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_time_less_than":
                    {
                        return new RoomItemWiredTimeLessThan(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_match_snapshot":
                    {
                        return new RoomItemWiredMatchSnapshop(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_trggrer_on_frn":
                    {
                        return new RoomItemWiredTriggererOnFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_furnis_hv_avtrs":
                    {
                        return new RoomItemWiredUsersStandOnFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_has_furni_on":
                    {
                        return new RoomItemWiredHasFurniOn(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "roller":
                    {
                        return new RoomItemRoller(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "gate":
                    {
                        return new RoomItemGate(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "blackhole":
                    {
                        return new RoomItemBlackHole(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_collision":
                    {
                        return new RoomItemWiredTriggerCollision(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_chase":
                    {
                        return new RoomItemWiredActionChase(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "onewaygate":
                    {
                        return new RoomItemOneWayGate(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "counter":
                    {
                        return new RoomItemCounter(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_tile":
                    {
                        return new RoomItemFreezeTile(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_exit_tile":
                    {
                        return new RoomItemFreezeExitTile(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_ice_block":
                    {
                        return new RoomItemFreezeIceBlock(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_gate_blue":
                    {
                        return new RoomItemFreezeGateBlue(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_gate_green":
                    {
                        return new RoomItemFreezeGateGreen(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_gate_red":
                    {
                        return new RoomItemFreezeGateRed(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_gate_yellow":
                    {
                        return new RoomItemFreezeGateYellow(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_score_yellow":
                    {
                        return new RoomItemFreezeScoreYellow(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_score_red":
                    {
                        return new RoomItemFreezeScoreRed(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_score_blue":
                    {
                        return new RoomItemFreezeScoreBlue(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "freeze_score_green":
                    {
                        return new RoomItemFreezeScoreGreen(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_trg_skylight":
                    {
                        return new RoomItemWiredTriggerSkylight(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room); ;
                    }
                case "wf_act_skylight":
                    {
                        return new RoomItemWiredActionSkylight(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_skylight":
                    {
                        return new RoomItemWiredConditionSkylight(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_call_stacks":
                    {
                        return new RoomItemWiredCallStack(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_join_team":
                    {
                        return new RoomItemWiredJoinTeam(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_leave_team":
                    {
                        return new RoomItemWiredLeaveTeam(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_actor_in_team":
                    {
                        return new RoomItemWiredInTeam(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "switch":
                    {
                        return new RoomItemWiredSwitch(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_teleport":
                    {
                        return new RoomItemBattleBanzaiRandomTeleport(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "puzzlebox":
                    {
                        return new RoomItemPuzzleBox(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "ball":
                    {
                        return new RoomItemBall(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_trgr_stacks":
                    {
                        return new RoomItemActionTriggerStacks(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_green_gate":
                    {
                        return new RoomItemBattleBanzaiGateGreen(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_red_gate":
                    {
                        return new RoomItemBattleBanzaiGateRed(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_yellow_gate":
                    {
                        return new RoomItemBattleBanzaiGateYellow(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_blue_gate":
                    {
                        return new RoomItemBattleBanzaiGateBlue(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_patch":
                    {
                        return new RoomItemBattleBanzaiPatch(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "bb_puck":
                    {
                        return new RoomItemBattleBanzaiPuck(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "blue_score":
                    {
                        return new RoomItemFootballBlueScore(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "green_score":
                    {
                        return new RoomItemFootballGreenScore(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "yellow_score":
                    {
                        return new RoomItemFootballYellowScore(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "red_score":
                    {
                        return new RoomItemFootballRedScore(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "fbgate":
                    {
                        return new RoomItemFootballGate(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_user_count_in":
                    {
                        return new RoomItemWiredUserCountIn(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_wearing_effect":
                    {
                        return new RoomItemWiredWearingEffect(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_wearing_badge":
                    {
                        return new RoomItemWiredWearingBadge(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_wearing_fx":
                    {
                        return new RoomItemWiredNotWearingEffect(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_wearing_b":
                    {
                        return new RoomItemWiredNotWearingBadge(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_user_count":
                    {
                        return new RoomItemWiredNotUserCountIn(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_hv_avtrs":
                    {
                        return new RoomItemWiredUsersNotStandOnFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_trggrer_on":
                    {
                        return new RoomItemWiredTriggererNotOnFurni(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_match_snap":
                    {
                        return new RoomItemWiredNotMatchSnapshop(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_give_score_tm":
                    {
                        return new RoomItemWiredGiveScoreTeam(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_move_to_dir":
                    {
                        return new RoomItemWiredMoveToDir(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "scoreboard":
                    {
                        return new RoomItemScoreboard(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_act_trigger":
                    {
                        return new RoomItemWiredActionTrigger(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_triggered":
                    {
                        return new RoomItemWiredTriggered(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "wf_cnd_not_triggered":
                    {
                        return new RoomItemWiredNotTriggered(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "firework":
                    {
                        return new RoomItemFirework(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "horse_obstacle":
                    {
                        return new RoomItemHorseObstacle(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "sb_rail":
                    {
                        return new RoomItemSkateboardRail(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "skating_ice":
                    {
                        return new RoomItemIceSkatingPatch(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "tagpole":
                    {
                        return new RoomItemTagPole(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "rollerskate":
                    {
                        return new RoomItemRollerskate(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "jukebox":
                    {
                        return new RoomItemJukebox(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "blue_goal":
                case "green_goal":
                case "yellow_goal":
                case "red_goal":
                    {
                        return new RoomItemFootballGoal(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "water":
                    {
                        return new RoomItemWater(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                case "photo":
                    {
                        return new RoomItemPhoto(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
                default:
                    {
                        return new RoomItem(id, roomId, userId, baseItem_, extraData, x, y, z, rot, wallCoordinate, room);
                    }
            }
        }

        public Item GetBaseItem()
        {
            return this.BaseItem;
        }

        public bool IsFloorItem
        {
            get
            {
                return this.BaseItem.Type == "s";
            }
        }

        public bool IsWallItem
        {
            get
            {
                return this.BaseItem.Type == "i";
            }
        }

        public void SetLocation(int x, int y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.CalcAffectedTiles();
        }

        public void Serialize(ServerMessage message)
        {
            if (this.IsFloorItem)
            {
                if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                {
                    message.AppendUInt(this.ID);
                    message.AppendInt32(this.BaseItem.SpriteID);
                }
                else
                {
                    message.AppendString(this.ID.ToString());
                    message.AppendString(this.BaseItem.ItemName);
                }
                message.AppendInt32(this.X);
                message.AppendInt32(this.Y);
                if (message.GetRevision() < Revision.RELEASE63_35255_34886_201108111108)
                {
                    message.AppendInt32(this.BaseItem.Lenght);
                    message.AppendInt32(this.BaseItem.Width);
                }
                message.AppendInt32(this.Rot);
                message.AppendString(TextUtilies.DoubleWithDotDecimal(this.Z));
                if (message.GetRevision() > Revision.RELEASE63_201211141113_913728051)
                {
                    message.AppendString(""); //What is this?
                }
                if (this.GetBaseItem().ItemName.StartsWith("present_"))
                {
                    string[] data = this.ExtraData.Split((char)9);
                    if (data.Length >= 2)
                    {
                        if (this.GetBaseItem().ItemName.StartsWith("present_wrap"))
                        {
                            message.AppendInt32(int.Parse(data[2]) * 1000 + int.Parse(data[3])); //gift style
                        }
                        else
                        {
                            message.AppendInt32(0);
                        }

                        message.AppendString("!" + data[0] + "\n\n-" + Skylight.GetGame().GetGameClientManager().GetUsernameByID(uint.Parse(data[1]))); //client ignores first char
                    }
                    else
                    {
                        message.AppendInt32(0);
                        message.AppendString("!" + data[0]);
                    }
                }
                else if (this.GetBaseItem().InteractionType.ToLower() == "trophy")
                {
                    message.AppendInt32(0);
                    string[] data = this.ExtraData.Split((char)9);
                    message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(uint.Parse(data[0])) + (char)9 + string.Join("" + (char)9, data, 1, data.Length - 1));
                }
                else
                {
                    if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                    {
                        if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                        {
                            message.AppendInt32(0); //case
                        }
                        message.AppendInt32(0); //song id, gift style, etc
                        message.AppendString(this.ExtraData);
                    }
                    else
                    {
                        message.AppendString(""); //COLOR, r63a doesnt have this
                        message.AppendString(""); //item status, for pets?
                        message.AppendBoolean(false); //is dis fucking pet food or nah
                        message.AppendString(OldSchoolUtils.GetOldSchoolExtraData(this));
                    }
                }

                if (message.GetRevision() > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                {
                    message.AppendInt32(-1); //expire time
                    if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                    {
                        message.AppendInt32(1); //use button
                        message.AppendUInt(this.UserID);
                    }
                    else
                    {
                        message.AppendBoolean(true); //use button
                    }

                    if (this.GetBaseItem().SpriteID < 0)
                    {
                        message.AppendString(this.GetBaseItem().ItemName); //if sprite id is negative then this is needed
                    }
                }
            }
            else
            {
                if (this.IsWallItem) //just for sure nothing goes wrong
                {
                    message.AppendString(this.ID.ToString());
                    message.AppendInt32(this.BaseItem.SpriteID);
                    message.AppendString(this.WallCoordinate != null ? this.WallCoordinate.ToString() : ""); //sometimes its null ;(

                    if (this.BaseItem.ItemName.StartsWith("poster_"))
                    {
                        message.AppendString(this.BaseItem.ItemName.Split(new char[]
						{
							'_'
						})[1]);
                    }
                    else
                    {
                        string text = this.BaseItem.InteractionType.ToLower();
                        if (text != null && text == "postit")
                        {
                            message.AppendString(this.ExtraData.Split(new char[]
						    {
							    ' '
						    })[0]);
                        }
                        else
                        {
                            message.AppendString(this.ExtraData);
                        }
                    }

                    if (message.GetRevision() < Revision.PRODUCTION_201601012205_226667486)
                    {
                        if (message.GetRevision() >= Revision.RELEASE63_201211141113_913728051)
                        {
                            message.AppendInt32(1); //use button
                            message.AppendUInt(this.UserID);
                        }
                        else
                        {
                            message.AppendBoolean(true); //use button
                        }
                    }
                    else
                    {
                        message.AppendUInt(this.UserID);
                        message.AppendInt32(1); //use button
                        message.AppendInt32(0); //idk
                    }
                }
            }
        }

        public void UpdateState(bool updateDatabase, bool updateRoom)
        {
            if (updateDatabase)
            {
                this.Room.RoomItemManager.UpdateItemStateToDatabase(this);
            }

            if (updateRoom)
            {
                this.TryChangeUpdateState(UpdateStatus.STATE);
                this.Room.RoomItemManager.RequireUpdateClientSide[this.ID] = this;
            }
        }

        public ThreeDCoord TDC
        {
            get
            {
                ThreeDCoord tdc = new ThreeDCoord(this.X, this.Y);
                if (this.Rot == 0)
                {
                    tdc.y--;
                }
                else
                {
                    if (this.Rot == 2)
                    {
                        tdc.x++;
                    }
                    else
                    {
                        if (this.Rot == 4)
                        {
                            tdc.y++;
                        }
                        else
                        {
                            if (this.Rot == 6)
                            {
                                tdc.x--;
                            }
                        }
                    }
                }

                return tdc;
            }
        }

        public ThreeDCoord TDCO
        {
            get
            {
                ThreeDCoord tdc = new ThreeDCoord(this.X, this.Y);
                if (this.Rot == 0)
                {
                    tdc.y++;
                }
                else
                {
                    if (this.Rot == 2)
                    {
                        tdc.x--;
                    }
                    else
                    {
                        if (this.Rot == 4)
                        {
                            tdc.y--;
                        }
                        else
                        {
                            if (this.Rot == 6)
                            {
                                tdc.x++;
                            }
                        }
                    }
                }

                return tdc;
            }
        }

        public ThreeDCoord TDCR(int rotation)
        {
            ThreeDCoord tdc = new ThreeDCoord(this.X, this.Y);
            if (rotation == 0)
            {
                tdc.y--;
            }
            else
            {
                if (rotation == 2)
                {
                    tdc.x++;
                }
                else
                {
                    if (rotation == 4)
                    {
                        tdc.y++;
                    }
                    else
                    {
                        if (rotation == 6)
                        {
                            tdc.x--;
                        }
                    }
                }
            }

            return tdc;
        }

        public virtual void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            int modesCount = this.BaseItem.InteractionModeCounts - 1;

            if (modesCount > 0)
            {
                if (userHasRights)
                {
                    int mode = 0;
                    int.TryParse(item.ExtraData, out mode); //if we fail mode stays at 0

                    int forceState = session != null ? session.GetHabbo().GetRoomSession().GetRoomUser().ForceState : - 1;
                    if (forceState != -1)
                    {
                        mode = forceState;

                        if (mode > modesCount)
                        {
                            mode = 0;
                        }
                    }
                    else
                    {
                        if (mode <= 0)
                        {
                            mode = 1;
                        }
                        else
                        {
                            if (mode >= modesCount)
                            {
                                mode = 0;
                            }
                            else
                            {
                                mode++;
                            }
                        }
                    }

                    double oldHeight = item.ActiveHeight;
                    item.ExtraData = mode.ToString();
                    item.UpdateState(true, true);
                    if (oldHeight != item.ActiveHeight)
                    {
                        foreach(RoomUnitUser user in this.Room.RoomUserManager.GetRoomUsers())
                        {
                            if (user.X == this.X && user.Y == this.Y)
                            {
                                user.UpdateState();
                            }
                            else
                            {
                                foreach(AffectedTile tile in this.AffectedTiles)
                                {
                                    if (user.X == tile.X && user.Y == tile.Y)
                                    {
                                        user.UpdateState();
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (session != null)
                    {
                        this.Room.RoomWiredManager.UseItem(session.GetHabbo().GetRoomSession().GetRoomUser(), this);
                    }
                }
            }
        }

        public double ActiveHeight
        {
            get
            {
                if (this.GetBaseItem().HeightAdjustable?.Length > 1)
                {
                    int index = 0;
                    int.TryParse(this.ExtraData, out index);
                    return this.Z + this.GetBaseItem().HeightAdjustable[index];
                }
                else
                {
                    return this.Z + this.BaseItem.Height;
                }
            }
        }

        public void DoUpdate(int timer)
        {
            this.UpdateTimer = timer;
            this.UpdateNeeded = true;
        }

        public virtual void OnCycle()
        {

        }

        //item placed by user
        public virtual void OnPlace(GameClient session)
        {

        }

        //item picked up by user
        public virtual void OnPickup(GameClient session)
        {

        }

        //on item moved, session null if it wasent user
        public virtual void OnMove(GameClient session)
        {

        }

        //on room loads the item
        public virtual void OnLoad()
        {

        }

        //THIS IS FOR WIREDS AND OTHER STUFF THAT STORES DATA THAT DOSENT BELONG TO "EXTRA DATA"
        public virtual string GetItemData()
        {
            return "";
        }

        //THIS IS FOR WIREDS AND OTHER STUFF THAT STORES DATA THAT DOSENT BELONG TO "EXTRA DATA"
        public virtual void LoadItemData(string data)
        {

        }

        public virtual void OnWalkOn(RoomUnit user)
        {

        }

        public virtual void OnWalkOff(RoomUnit user)
        {

        }

        public bool WiredIgnoreExtraData()
        {
            if (this.GetBaseItem().ItemName.StartsWith("present_") || this.GetBaseItem().InteractionType.ToLower() == "trophy")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void AboutToWalkOn(RoomUnit user)
        {

        }

        public void TryChangeUpdateState(UpdateStatus status)
        {
            if (status == UpdateStatus.NONE)
            {
                this.RoomUpdateStatus = status;
            }
            else
            {
                if (status > this.RoomUpdateStatus)
                {
                    this.RoomUpdateStatus = status;
                }
            }
        }

        public enum UpdateStatus
        {
            NONE = 0,
            STATE = 1,
            MOVE = 2,
        }
    }
}
