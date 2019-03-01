using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class RedeemFurniMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                RoomUnit user = session.GetHabbo().GetRoomSession().GetRoomUser();
                if (user != null && user.Room != null)
                {
                    uint itemId = message.PopWiredUInt();
                    RoomItem item = user.Room.RoomItemManager.TryGetRoomItem(itemId);
                    if (item != null)
                    {
                        if (item.GetBaseItem().ItemName.StartsWith("CF_") || item.GetBaseItem().ItemName.StartsWith("CFC_") || item.GetBaseItem().ItemName.StartsWith("PixEx_") || item.GetBaseItem().ItemName.StartsWith("PntEx_"))
                        {
                            user.Room.RoomItemManager.RemoveItem(session, item);

                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("itemId", item.ID);
                                dbClient.ExecuteQuery("DELETE FROM items WHERE id = @itemId LIMIT 1");
                            }

                            string[] array = item.GetBaseItem().ItemName.Split(new char[]
							{
								'_'
							});

                            if (!item.GetBaseItem().ItemName.StartsWith("PntEx_"))
                            {
                                int value = int.Parse(array[1]);

                                if (item.GetBaseItem().ItemName.StartsWith("CF_") || item.GetBaseItem().ItemName.StartsWith("CFC_"))
                                {
                                    session.GetHabbo().Credits += value;
                                    session.GetHabbo().UpdateCredits(true);
                                }
                                else
                                {
                                    if (item.GetBaseItem().ItemName.StartsWith("PixEx_"))
                                    {
                                        session.GetHabbo().AddActivityPoints(0, value);
                                        session.GetHabbo().UpdateActivityPoints(0, true);
                                    }
                                }
                            }
                            else //point excahange
                            {
                                int key = int.Parse(array[1]);
                                int value = int.Parse(array[2]);

                                session.GetHabbo().AddActivityPoints(key, value);
                                session.GetHabbo().UpdateActivityPoints(key, true);
                            }
                        }
                    }
                }
            }
        }
    }
}
