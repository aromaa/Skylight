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
    class RoomItemWiredSayMessage : RoomItemWiredAction
    {
        public string Message;

        public RoomItemWiredSayMessage(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Message = "";
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (userHasRights)
            {
                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.WiredAction);
                message.AppendBoolean(false); //check box toggling
                message.AppendInt32(0); //furni limit
                message.AppendInt32(0); //furni count
                message.AppendInt32(this.GetBaseItem().SpriteID); //sprite id, show the help thing
                message.AppendUInt(this.ID); //item id
                message.AppendString(this.Message); //data
                message.AppendInt32(0); //extra data count
                message.AppendInt32(0); //idk

                message.AppendInt32(7); //type
                message.AppendInt32(0); //delay, not work with this wired
                message.AppendInt32(0); //conflicts count
                session.SendMessage(message);
            }
        }

        public override string GetItemData()
        {
            return this.Message;
        }

        public override void LoadItemData(string data)
        {
            this.Message = data;
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
            if (this.Message.Length > 0)
            {
                if (triggerer != null)
                {
                    ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message.Init(r63aOutgoing.Whisper);
                    message.AppendInt32(triggerer.VirtualID);

                    string message_ = this.Message;
                    Dictionary<int, string> links = new Dictionary<int, string>();
                    if (message_.Contains("http://") || message_.Contains("www.") || message_.Contains("https://"))
                    {
                        string[] words = message_.Split(' ');
                        message_ = "";

                        foreach (string word in words)
                        {
                            if (TextUtilies.ValidURL(word))
                            {
                                int index = links.Count;
                                links.Add(index, word);

                                message_ += " {" + index + "}";
                            }
                            else
                            {
                                message_ += " " + word;
                            }
                        }
                    }
                    message.AppendString(message_);
                    message.AppendInt32(0); //gesture
                    message.AppendInt32(links.Count); //links count
                    foreach (KeyValuePair<int, string> link in links)
                    {
                        message.AppendString("/redirect.php?url=" + link.Value);
                        message.AppendString(link.Value);
                        message.AppendBoolean(true); //trushed, can link be opened
                    }
                    triggerer.Session.SendMessage(message);
                }
                else
                {
                    foreach (RoomUnitUser user in this.Room.RoomUserManager.GetRealUsers())
                    {
                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message.Init(r63aOutgoing.Whisper);
                        message.AppendInt32(user.VirtualID);

                        string message_ = this.Message;
                        Dictionary<int, string> links = new Dictionary<int, string>();
                        if (message_.Contains("http://") || message_.Contains("www.") || message_.Contains("https://"))
                        {
                            string[] words = message_.Split(' ');
                            message_ = "";

                            foreach (string word in words)
                            {
                                if (TextUtilies.ValidURL(word))
                                {
                                    int index = links.Count;
                                    links.Add(index, word);

                                    message_ += " {" + index + "}";
                                }
                                else
                                {
                                    message_ += " " + word;
                                }
                            }
                        }
                        message.AppendString(message_);
                        message.AppendInt32(0); //gesture
                        message.AppendInt32(links.Count); //links count
                        foreach (KeyValuePair<int, string> link in links)
                        {
                            message.AppendString("/redirect.php?url=" + link.Value);
                            message.AppendString(link.Value);
                            message.AppendBoolean(true); //trushed, can link be opened
                        }
                        user.Session.SendMessage(message);
                    }
                }
            }
        }
    }
}
