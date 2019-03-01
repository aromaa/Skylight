using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;

namespace SkylightEmulator.Communication.Messages.Incoming.r26
{
    class CreateRoomPhaseTwoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            string[] data = message.PopStringUntilBreak(null).Split(Convert.ToChar(13));

            uint roomId = 0;
            string[] roomIdS = data[0].Split('/');
            if (!uint.TryParse(roomIdS[0], out roomId))
            {
                uint.TryParse(roomIdS[1], out roomId);
            }

            RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
            if (roomData != null && roomData.OwnerID == session.GetHabbo().ID && data.Length > 1)
            {
                string query = "UPDATE rooms SET ";

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    for (int i = 1; i < data.Length; i++)
                    {
                        string[] data_ = data[i].Split(new char[] { '=' }, 2);

                        string key = data_[0];
                        string value = data_[1];

                        switch (key)
                        {
                            case "description":
                                {
                                    if (i > 1)
                                    {
                                        query += ", ";
                                    }
                                    dbClient.AddParamWithValue("description", value);
                                    query += "description = @description";

                                    roomData.Description = value;
                                }
                                break;
                            case "password":
                                {
                                    if (i > 1)
                                    {
                                        query += ", ";
                                    }
                                    dbClient.AddParamWithValue("password", value);
                                    query += "password = @password";

                                    roomData.Password = value;
                                }
                                break;
                            case "allsuperuser":
                                {
                                    //IMPLEMENT
                                }
                                break;
                        }
                    }
                    
                    dbClient.ExecuteQuery(query + " WHERE id = '" + roomId + "' LIMIT 1");
                }
            }
        }
    }
}
