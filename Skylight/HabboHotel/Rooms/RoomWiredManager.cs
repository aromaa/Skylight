using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Pathfinders;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomWiredManager
    {
        private Room Room;
        public int TimerCycle;
        public List<RoomWiredDelay> WiredDelays;

        public RoomWiredManager(Room room)
        {
            this.Room = room;
            this.TimerCycle = 0;
            this.WiredDelays = new List<RoomWiredDelay>();
        }

        public void UserEnterRoom(RoomUnitUser user)
        {
            foreach (RoomItemWiredTrigger item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredEnterRoom)))
            {
                this.WiredTrigger(item, user, new HashSet<uint>());
            }
        }

        public void OnCycle()
        {
            this.TimerCycle++;

            if (this.WiredDelays.Count > 0)
            {
                foreach (RoomWiredDelay delay in this.WiredDelays.ToList())
                {
                    this.Room.ThrowIfRoomCycleCancalled("Cycle room wired items", delay); //Have room cycle cancalled?

                    delay.Delay--;
                    if (delay.Delay <= 0)
                    {
                        if (delay.Wired is RoomItemWiredAction)
                        {
                            delay.Wired.DoWiredAction(delay.Triggerer, delay.Used);
                        }
                        else
                        {
                            //Why?!?!?
                        }

                        this.WiredDelays.Remove(delay);
                    }
                }
            }
        }

        public bool WiredTrigger(RoomItem trigger, RoomUnitUser triggerer, HashSet<uint> used, object extraData = null)
        {
            uint wiredId = trigger?.ID ?? (extraData as RoomItem)?.ID ?? 0; //Love the one line <3

            if (wiredId > 0 && used.Add(wiredId)) //prevent stackoverflow :O :)
            {
                if (extraData is RoomItemWiredCallStack || extraData is RoomItemActionTriggerStacks || (trigger as RoomItemWiredTrigger)?.TryTrigger(triggerer, extraData) == true) //it got triggered
                {
                    bool conditionBlocking = false;

                    IEnumerable<RoomItem> allTileItems = this.Room.RoomGamemapManager.GetTile(trigger.X, trigger.Y).ItemsOnTile.Values.OrderBy(i => i.Z);
                    if (!(extraData is RoomItemWiredCallStack))
                    {
                        trigger.ExtraData = "1";
                        trigger.UpdateState(false, true);
                        trigger.DoUpdate(1);

                        foreach (RoomItemWiredCondition item in allTileItems.Where(i => this.IsWiredCondition(i)))
                        {
                            if (conditionBlocking = item.IsBlocking(triggerer))
                            {
                                break;
                            }
                            else
                            {
                                item.ExtraData = "1";
                                item.UpdateState(false, true);
                                item.DoUpdate(1);
                            }
                        }
                    }

                    if (!conditionBlocking) //conditions are fine
                    {
                        List<RoomItem> actionStack = allTileItems.Where(i => this.IsWiredAction(i)).ToList(); //we need get all actions ready

                        if (actionStack.Count > 1) //there is more then one so we should actually do something
                        {
                            foreach (RoomItem item in allTileItems.Where(i => this.IsWiredExtra(i))) //only one works
                            {
                                if (item is RoomItemWiredRandom)
                                {
                                    RoomItem randomWired = actionStack[RandomUtilies.GetRandom(0, actionStack.Count - 1)];

                                    actionStack.Clear();
                                    actionStack.Add(randomWired);
                                }
                                else if (item is RoomItemWiredUnseen wired)
                                {
                                    if (wired.UnUsedWireds != null && wired.UnUsedWireds.Count > 0)
                                    {
                                        RoomItem nextWired = wired.UnUsedWireds.First();
                                        actionStack.Clear();
                                        actionStack.Add(nextWired);
                                        wired.UnUsedWireds.Remove(nextWired);
                                    }
                                    else
                                    {
                                        wired.UnUsedWireds = actionStack.ToList();

                                        RoomItem nextWired = wired.UnUsedWireds.First();
                                        actionStack.Clear();
                                        actionStack.Add(nextWired);
                                        wired.UnUsedWireds.Remove(nextWired);
                                    }
                                }
                                else
                                {
                                    continue; //???? why
                                }

                                item.ExtraData = "1";
                                item.UpdateState(false, true);
                                item.DoUpdate(1);

                                break;
                            }
                        }

                        //do the actions
                        foreach (RoomItemWiredAction item in actionStack)
                        {
                            if (item.Delay > 0)
                            {
                                this.WiredDelays.Add(new RoomWiredDelay(item, triggerer, item.Delay, used));
                            }
                            else
                            {
                                item.DoWiredAction(triggerer, used);
                            }

                            item.ExtraData = "1";
                            item.UpdateState(false, true);
                            item.DoUpdate(1);
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsWiredCondition(RoomItem item)
        {
            return item is RoomItemWiredCondition;
        }

        public bool IsWiredExtra(RoomItem item)
        {
            return item is RoomItemWiredRandom || item is RoomItemWiredUnseen;
        }

        public bool IsWiredAction(RoomItem item)
        {
            return item is RoomItemWiredAction;
        }

        public void UserWalkOff(RoomUnit user)
        {
            if (user.IsRealUser && user is RoomUnitUser user_)
            {
                RoomTile tile = user.CurrentTile;
                if (tile?.HigestRoomItem != null)
                {
                    foreach (RoomItemWiredTrigger item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredOffFurni)))
                    {
                        this.WiredTrigger(item, user_, new HashSet<uint>(), tile.HigestRoomItem.ID);
                    }
                }
            }
        }

        public void UserWalkOn(RoomUnit user)
        {
            if (user.IsRealUser && user is RoomUnitUser user_)
            {
                RoomTile tile = user.CurrentTile;
                if (tile?.HigestRoomItem != null)
                {
                    foreach (RoomItemWiredTrigger item in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredOnFurni)))
                    {
                        this.WiredTrigger(item, user_, new HashSet<uint>(), tile.HigestRoomItem.ID);
                    }
                }
            }
        }

        public bool UserSpeak(RoomUnitUser user, string message)
        {
            bool result = false;

            List<RoomItem> wireds = this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredOnSay));
            if (wireds != null)
            {
                foreach (RoomItemWiredTrigger item in wireds)
                {
                    if (this.WiredTrigger(item, user, new HashSet<uint>(), message))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public void UseItem(RoomUnitUser user, RoomItem item)
        {
            foreach (RoomItemWiredTrigger item_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredFurniState)))
            {
                this.WiredTrigger(item_, user, new HashSet<uint>(), item.ID);
            }
        }

        public void UserCollide(RoomUnitUser user, RoomItem item, HashSet<uint> used)
        {
            foreach (RoomItemWiredTrigger item_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredTriggerCollision)))
            {
                this.WiredTrigger(item_, user, used, item);
            }
        }

        public void GameStart()
        {
            foreach (RoomItemWiredTrigger item_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredGameStart)))
            {
                this.WiredTrigger(item_, null, new HashSet<uint>());
            }
            
            foreach (RoomItemWiredGivePoints item_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredGivePoints)))
            {
                item_.PointsAmountUsed = 0;
            }
        }

        public void GameEnd()
        {
            foreach (RoomItemWiredTrigger item_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredGameEnd)))
            {
                this.WiredTrigger(item_, null, new HashSet<uint>());
            }
        }

        public void ScoreChanged(RoomUnitUser triggerer, int newScore)
        {
            foreach (RoomItemWiredTrigger item_ in this.Room.RoomItemManager.FloorItems.Get(typeof(RoomItemWiredAtScore)))
            {
                this.WiredTrigger(item_, triggerer, new HashSet<uint>(), newScore);
            }
        }

        public void ExecuteWiredStacks(RoomUnitUser user, RoomItem item, RoomItem caller, HashSet<uint> used)
        {
            this.WiredTrigger(item, user, used, caller);
        }

        public void TriggerWiredStacks(RoomUnitUser user, RoomItem item, RoomItem caller, HashSet<uint> used)
        {
            this.WiredTrigger(item, user, used, caller);
        }
    }
}
