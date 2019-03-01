using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class BuyRoomCommand : Command
    {
        private static readonly Dictionary<string, int> Currencys = new Dictionary<string, int>() { { "pixels", 0 }, { "hearts", 1 }, { "snowflakes", 2 }, { "shells", 3 }, { "giftpoints", 4 } };

        public override string CommandInfo()
        {
            return ":buyroom - Invest your money in this room";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.SellRoomPrice.Count > 0)
            {
                bool canBuy = true;
                foreach(KeyValuePair<string, int> prize in session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.SellRoomPrice)
                {
                    if (prize.Key == "credits")
                    {
                        if (session.GetHabbo().Credits - prize.Value < 0)
                        {
                            session.SendNotif("You don't have enought credits! You need " + -(session.GetHabbo().Credits - prize.Value) + " more");
                            canBuy = false;
                            break;
                        }
                    }
                    else
                    {
                        if (BuyRoomCommand.Currencys.TryGetValue(prize.Key, out int currencyId))
                        {
                            if (session.GetHabbo().TryGetActivityPoints(currencyId) - prize.Value < 0)
                            {
                                session.SendNotif("You don't have enought " + prize.Key + "! You need " + -(session.GetHabbo().TryGetActivityPoints(currencyId) - prize.Value) + " more");
                                canBuy = false;
                                break;
                            }
                        }
                    }
                }

                if (canBuy)
                {
                    GameClient owner = Skylight.GetGame().GetGameClientManager().GetGameClientById(session.GetHabbo().GetRoomSession().GetRoom().RoomData.OwnerID);
                    string ownerCredits = null;
                    Dictionary<int, int> ownerActivityPoints = null;
                    if (owner == null)
                    {
                        ownerActivityPoints = new Dictionary<int, int>();

                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            string activityPoints = (string)dbClient.ReadDataRow("SELECT activity_points FROM users WHERE id = " + session.GetHabbo().GetRoomSession().GetRoom().RoomData.OwnerID + " LIMIT 1")[0];
                            if (!int.TryParse(activityPoints, out int pixels))
                            {
                                foreach (string s in activityPoints.Split(';'))
                                {
                                    string[] activityPointsData = s.Split(',');

                                    if (int.TryParse(activityPointsData[0], out int activityPointId))
                                    {
                                        if (int.TryParse(activityPointsData[1], out int activityPointAmount))
                                        {
                                            ownerActivityPoints.Add(activityPointId, activityPointAmount);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ownerActivityPoints.Add(0, pixels);
                            }
                        }
                    }

                    foreach (KeyValuePair<string, int> prize in session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.SellRoomPrice)
                    {
                        if (prize.Key == "credits")
                        {
                            session.GetHabbo().Credits -= prize.Value;
                            session.GetHabbo().UpdateCredits(true);

                            if (owner != null)
                            {
                                owner.GetHabbo().Credits += prize.Value;
                                owner.GetHabbo().UpdateCredits(true);
                            }
                            else
                            {
                                ownerCredits = "UPDATE users SET credits = credits " + prize.Value + " WHERE id = " + session.GetHabbo().GetRoomSession().GetRoom().RoomData.OwnerID + " LIMIT 1";
                            }
                        }
                        else
                        {
                            if (BuyRoomCommand.Currencys.TryGetValue(prize.Key, out int currencyId))
                            {
                                session.GetHabbo().RemoveActivityPoints(currencyId, prize.Value);
                                session.GetHabbo().UpdateActivityPoints(currencyId, true);

                                if (owner != null)
                                {
                                    owner.GetHabbo().AddActivityPoints(currencyId, prize.Value);
                                    owner.GetHabbo().UpdateActivityPoints(currencyId, true);
                                }
                                else
                                {
                                    if (ownerActivityPoints.TryGetValue(currencyId, out int amount))
                                    {
                                        ownerActivityPoints[currencyId] += prize.Value;
                                    }
                                    else
                                    {
                                        ownerActivityPoints.Add(currencyId, prize.Value);
                                    }
                                }
                            }
                        }
                    }

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("newOwnerId", session.GetHabbo().ID);
                        dbClient.AddParamWithValue("roomId", session.GetHabbo().GetRoomSession().GetRoom().ID);
                        dbClient.ExecuteQuery("UPDATE rooms SET ownerid = @newOwnerId WHERE id = @roomId LIMIT 1");

                        if (!string.IsNullOrEmpty(ownerCredits))
                        {
                            dbClient.ExecuteQuery(ownerCredits);
                        }

                        if (ownerActivityPoints != null)
                        {
                            string activityPointsData = "";
                            foreach (KeyValuePair<int, int> data in ownerActivityPoints)
                            {
                                if (activityPointsData.Length > 0)
                                {
                                    activityPointsData += ";";
                                }

                                activityPointsData += data.Key + "," + data.Value;
                            }

                            dbClient.ExecuteQuery(activityPointsData);
                        }
                    }

                    session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.SellRoomPrice = new Dictionary<string,int>();
                    foreach (RoomUnitUser user in session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.GetRealUsers())
                    {
                        try
                        {
                            if (user.Session.GetHabbo().ID != session.GetHabbo().ID)
                            {
                                user.Session.SendNotif("Someone just bought this room!");
                            }
                            else
                            {
                                user.Session.SendNotif("You just bought this room!");
                            }
                        }
                        catch
                        {

                        }
                    }
                    Skylight.GetGame().GetRoomManager().UnloadRoom(session.GetHabbo().GetRoomSession().GetRoom());
                }
            }
            else
            {
                session.SendNotif("This room isin't for sale");
            }

            return true;
        }
    }
}
