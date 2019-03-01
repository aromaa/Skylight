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

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class SaveRoomSettingsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session != null && session.GetHabbo() != null && session.GetHabbo().GetRoomSession() != null)
            {
                uint roomId = message.PopWiredUInt();

                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().GetRoomSession().CurrentRoomID);
                if (room != null && room.HaveOwnerRights(session) && room.ID == roomId)
                {
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
                    if (state < 0 || state > 3)
                    {
                        state = (int)room.RoomData.State;
                    }

                    string password = TextUtilies.FilterString(message.PopFixedString());
                    if (password.Length > 30)
                    {
                        password = room.RoomData.Password;
                    }

                    int usersMax = message.PopWiredInt32();
                    if (usersMax < 0 || usersMax % 5 == 1 || usersMax > 100)
                    {
                        usersMax = room.RoomData.UsersMax;
                    }

                    int category = message.PopWiredInt32();
                    if (Skylight.GetGame().GetNavigatorManager().GetFlatCat(category) == null) //invalid category
                    {
                        category = room.RoomData.Category; // 0 = no category
                    }

                    string tags = "";
                    int tagsCount = message.PopWiredInt32();
                    if (tagsCount >= 0 && tagsCount <= 2)
                    {
                        for (int i = 0; i < tagsCount; i++)
                        {
                            if (i > 0)
                            {
                                tags += ",";
                            }

                            tags += TextUtilies.CheckBlacklistedWords(TextUtilies.FilterString(message.PopFixedString()));
                        }

                        if (tags.Length > 100)
                        {
                            tags = string.Join(",", room.RoomData.Tags);
                        }
                    }
                    else
                    {
                        tags = string.Join(",", room.RoomData.Tags);
                    }

                    int allowTrade = message.PopWiredInt32();
                    if (allowTrade < 0 || allowTrade > 2)
                    {
                        allowTrade = (int)room.RoomData.AllowTrade;
                    }

                    bool allowPets = message.PopWiredBoolean();
                    bool allowPetsEat = message.PopWiredBoolean();
                    bool allowWalkthrough = message.PopWiredBoolean();
                    bool hidewalls = message.PopWiredBoolean();

                    int wallthick = message.PopWiredInt32();
                    if (wallthick < -2 || wallthick > 1)
                    {
                        wallthick = room.RoomData.Wallthick;
                    }

                    int floorthick = message.PopWiredInt32();
                    if (floorthick < -2 || floorthick > 1)
                    {
                        floorthick = room.RoomData.Floorthick;
                    }

                    int muteOption = message.PopWiredInt32();
                    if (muteOption < 0 || muteOption > 1)
                    {
                        muteOption = (int)room.RoomData.MuteOption;
                    }

                    int kickOption = message.PopWiredInt32();
                    if (kickOption < 0 || kickOption > 2)
                    {
                        kickOption = (int)room.RoomData.KickOption;
                    }

                    int banOption = message.PopWiredInt32();
                    if (banOption < 0 || banOption > 1)
                    {
                        banOption = (int)room.RoomData.BanOption;
                    }

                    int chatMode = message.PopWiredInt32();
                    if (chatMode < 0 || chatMode > 1)
                    {
                        chatMode = (int)room.RoomData.ChatMode;
                    }

                    int chatWeight = message.PopWiredInt32();
                    if (chatWeight < 0 || chatWeight > 2)
                    {
                        chatWeight = (int)room.RoomData.ChatWeight;
                    }

                    int chatSpeed = message.PopWiredInt32();
                    if (chatSpeed < 0 || chatSpeed > 2)
                    {
                        chatSpeed = (int)room.RoomData.ChatSpeed;
                    }

                    int chatDistance = message.PopWiredInt32();
                    if (chatDistance < 0 || chatDistance > 99)
                    {
                        chatDistance = room.RoomData.ChatDistance;
                    }

                    int chatProtection = message.PopWiredInt32();
                    if (chatProtection < 0 || chatProtection > 2)
                    {
                        chatProtection = (int)room.RoomData.ChatProtection;
                    }

                    room.RoomData.Name = name;
                    room.RoomData.Description = description;
                    room.RoomData.State = (RoomStateType)state;
                    room.RoomData.Password = password;
                    room.RoomData.UsersMax = usersMax;
                    room.RoomData.Category = category;
                    room.RoomData.Tags = tags.Split(',').ToList();
                    room.RoomData.AllowTrade = (RoomAllowTradeType)allowTrade;
                    room.RoomData.AllowPets = allowPets;
                    room.RoomData.AllowPetsEat = allowPetsEat;
                    room.RoomData.AllowWalkthrough = allowWalkthrough;
                    room.RoomData.Hidewalls = hidewalls;
                    room.RoomData.Wallthick = wallthick;
                    room.RoomData.Floorthick = floorthick;
                    room.RoomData.MuteOption = (RoomWhoCanType)muteOption;
                    room.RoomData.KickOption = (RoomWhoCanType)kickOption;
                    room.RoomData.BanOption = (RoomWhoCanType)banOption;
                    room.RoomData.ChatMode = (RoomChatModeType)chatMode;
                    room.RoomData.ChatWeight = (RoomChatWeightType)chatWeight;
                    room.RoomData.ChatSpeed = (RoomChatSpeedType)chatSpeed;
                    room.RoomData.ChatDistance = chatDistance;
                    room.RoomData.ChatProtection = (RoomChatProtectionType)chatProtection;

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
                        dbClient.AddParamWithValue("allow_pets", allowPets ? "1" : "0");
                        dbClient.AddParamWithValue("allow_pets_eat", allowPetsEat ? "1" : "0");
                        dbClient.AddParamWithValue("allow_walkthrough", allowWalkthrough ? "1" : "0");
                        dbClient.AddParamWithValue("hidewalls", hidewalls ? "1" : "0");
                        dbClient.AddParamWithValue("wallthick", wallthick);
                        dbClient.AddParamWithValue("floorthick", floorthick);

                        dbClient.ExecuteQuery("UPDATE rooms SET name = @name, description = @description, state = @state, password = @password, users_max = @users_max, category = @category, tags = @tags, allow_pets = @allow_pets, allow_pets_eat = @allow_pets_eat, allow_walkthrough = @allow_walkthrough, hidewalls = @hidewalls, wallthick = @wallthick, floorthick = @floorthick, trade = '" + allowTrade + "', mute_option = '" + muteOption + "', kick_option = '" + kickOption + "', ban_option = '" + banOption + "', chat_mode = '" + chatMode + "', chat_weight = '" + chatWeight + "', chat_speed = '" + chatSpeed + "', chat_protection = '" + chatProtection + "' WHERE id = @roomid LIMIT 1");
                    }

                    room.SendToAll(OutgoingPacketsEnum.RoomSettingsOK, new ValueHolder("RoomID", room.ID));
                    room.SendToAll(OutgoingPacketsEnum.RoomUpdateOK, new ValueHolder("RoomID", room.ID));
                    room.SendToAll(OutgoingPacketsEnum.RoomVIPSettings, new ValueHolder("Hidewalls", hidewalls, "Wallthick", wallthick, "Floorthick", floorthick));
                    room.SendToAll(OutgoingPacketsEnum.RoomChatSettings, new ValueHolder("ChatMod", chatMode, "ChatWeight", chatWeight, "ChatSpeed", chatSpeed, "ChatDistance", chatDistance, "ChatProtection", chatProtection));
                    room.SendToAll(OutgoingPacketsEnum.RoomData, new ValueHolder("Room", room.RoomData));
                }
            }
        }
    }
}
