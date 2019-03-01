using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class SaveRoomSettingsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                Room room = Skylight.GetGame().GetRoomManager().GetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null && room.IsOwner(session))
                {
                    uint roomId = message.PopWiredUInt();
                    string name = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(message.PopFixedString()));
                    if (name.Length > 100)
                    {
                        name = name.Substring(0, 100);
                    }

                    string description = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(message.PopFixedString()));
                    if (description.Length > 255)
                    {
                        description = description.Substring(0, 255);
                    }

                    int state = message.PopWiredInt32();
                    if (state < 0 || state > 2)
                    {
                        return;
                    }

                    string password = TextUtilies.FilterString(message.PopFixedString());
                    if (password.Length > 30)
                    {
                        password = "";
                    }

                    int usersMax = message.PopWiredInt32();
                    if (usersMax < 0 || usersMax % 5 == 1 || usersMax > 100)
                    {
                        return;
                    }

                    int category = message.PopWiredInt32();
                    if (category != 0) //only 0 avaible currently
                    {
                        category = 0; // 0 = no category
                    }

                    int tagsCount = message.PopWiredInt32();
                    if (tagsCount > 2)
                    {
                        return;
                    }

                    string tags = "";
                    for (int i = 0; i < tagsCount; i++)
                    {
                        if (i > 0)
                        {
                            tags += ",";
                        }

                        string tag = TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(message.PopFixedString()));
                        tags += tag;
                    }

                    if (tags.Length > 100)
                    {
                        tags = "";
                    }

                    bool allowPets = message.PopBase64Boolean();
                    bool allowPetsEat = message.PopBase64Boolean();
                    bool allowWalkthrough = message.PopBase64Boolean();
                    bool hidewalls = message.PopBase64Boolean();

                    int wallthick = message.PopWiredInt32();
                    if (wallthick < -2 || wallthick > 1)
                    {
                        wallthick = 0;
                    }

                    int floorthick = message.PopWiredInt32();
                    if (floorthick < -2 || floorthick > 1)
                    {
                        floorthick = 0;
                    }

                    room.RoomData.Name = name;
                    room.RoomData.Description = description;
                    room.RoomData.State = (RoomStateType)state;
                    room.RoomData.Password = password;
                    room.RoomData.UsersMax = usersMax;
                    room.RoomData.Category = category;
                    room.RoomData.Tags = tags.Split(',').ToList();
                    room.RoomData.AllowPets = allowPets;
                    room.RoomData.AllowPetsEat = allowPetsEat;
                    room.RoomData.AllowWalkthrough = allowWalkthrough;
                    room.RoomData.Hidewalls = hidewalls;
                    room.RoomData.Wallthick = wallthick;
                    room.RoomData.Floorthick = floorthick;

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("roomid", room.ID);
                        dbClient.AddParamWithValue("name", name);
                        dbClient.AddParamWithValue("description", description);
                        dbClient.AddParamWithValue("state", room.RoomData.State == RoomStateType.PASSWORD ? "password" : room.RoomData.State == RoomStateType.LOCKED ? "locked" : "open");
                        dbClient.AddParamWithValue("password", password);
                        dbClient.AddParamWithValue("users_max", usersMax);
                        dbClient.AddParamWithValue("category", category);
                        dbClient.AddParamWithValue("tags", tags);
                        dbClient.AddParamWithValue("allow_pets", allowPets ? 1 : 0);
                        dbClient.AddParamWithValue("allow_pets_eat", allowPetsEat ? 1 : 0);
                        dbClient.AddParamWithValue("allow_walkthrough", allowWalkthrough ? 1 : 0);
                        dbClient.AddParamWithValue("hidewalls", hidewalls ? 1 : 0);
                        dbClient.AddParamWithValue("wallthick", wallthick);
                        dbClient.AddParamWithValue("floorthick", floorthick);

                        dbClient.ExecuteQuery("UPDATE rooms SET name = @name, description = @description, state = @state, password = @password, users_max = @users_max, category = @category, tags = @tags, allow_pets = @allow_pets, allow_pets_eat = @allow_pets_eat, allow_walkthrough = @allow_walkthrough, hidewalls = @hidewalls, wallthick = @wallthick, floorthick = @floorthick WHERE id = @roomid LIMIT 1");
                    }

                    ServerMessage roomSettingsOK = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    roomSettingsOK.Init(r63aOutgoing.RoomSettingsOK);
                    roomSettingsOK.AppendUInt(room.ID);
                    room.SendToAll(roomSettingsOK, null);

                    ServerMessage roomUpdateOK = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    roomUpdateOK.Init(r63aOutgoing.RoomUpdateOK);
                    roomUpdateOK.AppendUInt(room.ID);
                    room.SendToAll(roomUpdateOK, null);

                    ServerMessage roomVIPsetting = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    roomVIPsetting.Init(r63aOutgoing.RoomVIPSettings);
                    roomVIPsetting.AppendBoolean(room.RoomData.Hidewalls);
                    roomVIPsetting.AppendInt32(room.RoomData.Wallthick);
                    roomVIPsetting.AppendInt32(room.RoomData.Floorthick);
                    room.SendToAll(roomVIPsetting, null);

                    ServerMessage roomData = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    roomData.Init(r63aOutgoing.RoomData);
                    roomData.AppendBoolean(false); //entered room
                    room.RoomData.Serialize(roomData, false);
                    roomData.AppendBoolean(false); //forward
                    roomData.AppendBoolean(false); //is staff pick
                    room.SendToAll(roomData, null);
                }
            }
        }
    }
}
