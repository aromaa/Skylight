using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class SellRoomCommand : Command
    {
        public override string CommandInfo()
        {
            return ":sellroom [currency:value] ... - Sell room";
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
            if (session.GetHabbo().ID == session.GetHabbo().GetRoomSession().GetRoom().RoomData.OwnerID)
            {
                if (args.Length >= 2)
                {
                    Dictionary<string, int> currencys = new Dictionary<string, int>();
                    for(int i = 1; i < args.Length; i++)
                    {
                        string[] data = args[i].Split(':');
                        string currency = data[0];
                        if (!this.ValidCurrencyName(currency))
                        {
                            session.SendNotif("Valid currencys are:\ncredits\npixels\nshearts\nsnowflakes\nshells\ngiftpoints");
                        }
                        else
                        {
                            int amount = int.Parse(data[1]);
                            if (amount < 0)
                            {
                                session.SendNotif("Only positive numbers!");
                                return true;
                            }
                            else
                            {
                                currencys.Add(currency, amount);
                            }
                        }
                    }

                    session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.SellRoomPrice = currencys;
                    session.SendNotif("Your room is now on sale!");
                }
                else
                {
                    session.SendNotif("Valid currencys are:\ncredits\npixels\nshearts\nsnowflakes\nshells\ngiftpoints");
                }

                return true;
            }

            return false;
        }

        public bool ValidCurrencyName(string currencyName)
        {
            if (currencyName == "credits" || currencyName == "pixels" || currencyName == "hearts" || currencyName == "snowflakes" || currencyName == "shells" || currencyName == "giftpoints")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
