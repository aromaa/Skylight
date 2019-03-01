using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomItemJukebox : RoomItem
    {
        public bool IsPlaying;
        public int SongQueuePosition;
        public List<MusicDisc> Playlist;
        public double SongStarted;

        public RoomItemJukebox(uint id, uint roomId, uint userId, Item baseItem, string extraData, int x, int y, double z, int rot, WallCoordinate wallCoordinate, Room room)
            : base(id, roomId, userId, baseItem, extraData, x, y, z, rot, wallCoordinate, room)
        {
            this.Playlist = new List<MusicDisc>();
        }

        public override void OnUse(GameClient session, RoomItem item, int request, bool userHasRights)
        {
            if (request >= 0) //playlist
            {
                if (this.ExtraData == "1")
                {
                    this.Stop();
                }
                else
                {
                    this.Start(request);
                }
            }
            else if (request == -1) //open jukebox
            {

            }
        }

        public override void OnPlace(GameClient session)
        {
            DataRow dataRow = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("itemId", this.ID);
                dataRow = dbClient.ReadDataRow("SELECT data FROM items_data WHERE item_id = @itemId LIMIT 1");
            }

            if (dataRow != null)
            {
                this.LoadItemData((string)dataRow["data"]);
            }
        }

        public override void LoadItemData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                string[] splitData = data.Split('|');
                foreach(string itemData in splitData)
                {
                    string[] itemDataSplit = itemData.Split(',');

                    uint itemId = uint.Parse(itemDataSplit[0]);
                    uint baseItem = uint.Parse(itemDataSplit[1]);
                    int songId = int.Parse(itemDataSplit[2]);
                    this.Playlist.Add(new MusicDisc(itemId, baseItem, songId));
                }
            }
            
            if (this.ExtraData == "1")
            {
                this.Start(0);
            }
        }

        public override string GetItemData()
        {
            string data = "";
            foreach(MusicDisc disc in this.Playlist)
            {
                if (data.Length > 0)
                {
                    data += "|";
                }
                data += disc.ItemId + "," + disc.BaseItem + "," + disc.SongID;
            }
            return data;
        }

        public override void OnPickup(GameClient session)
        {
            if (this.Room.RoomItemManager.ItemDataChanged.ContainsKey(this.ID))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("itemId", this.ID);
                    dbClient.AddParamWithValue("data", this.GetItemData());
                    dbClient.ExecuteQuery("INSERT INTO items_data(item_id, data) VALUES(@itemId, @data) ON DUPLICATE KEY UPDATE data = VALUES(data);");
                }
            }
        }

        public int PlaylistCapacity
        {
            get
            {
                return 20;
            }
        }

        public void AddDisc(MusicDisc disc)
        {
            this.Playlist.Add(disc);
            this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);
        }

        public MusicDisc RemoveDisc(int playlistIndex)
        {
            if (this.Playlist.Count > playlistIndex)
            {
                MusicDisc disc = this.Playlist[playlistIndex];
                this.Playlist.RemoveAt(playlistIndex);
                this.Room.RoomItemManager.ItemDataChanged.AddOrUpdate(this.ID, this, (key, oldValue) => this);

                if (this.SongQueuePosition == playlistIndex)
                {
                    this.NextSong();
                }
                return disc;
            }
            else
            {
                return null;
            }
        }

        public void Start(int request)
        {
            this.SongQueuePosition = request;
            this.PlaySong();

            this.ExtraData = "1";
            this.UpdateState(true, true);
        }

        public void NextSong()
        {
            this.SongQueuePosition++;
            this.PlaySong();
        }

        public MusicDisc GetCurrentSong()
        {
            if (this.Playlist.Count > this.SongQueuePosition)
            {
                return this.Playlist[this.SongQueuePosition];
            }
            else
            {
                return null;
            }
        }

        public void PlaySong()
        {
            if (this.Playlist.Count != 0)
            {
                this.IsPlaying = true;
                if (this.SongQueuePosition >= this.Playlist.Count)
                {
                    this.SongQueuePosition = 0;
                }

                this.SongStarted = TimeUtilies.GetUnixTimestamp();
                this.Room.SendToAll(this.GetSongData());
            }
            else
            {
                this.Stop();
            }
        }

        public void Stop()
        {
            this.IsPlaying = false;

            this.ExtraData = "0";
            this.UpdateState(true, true);

            this.Room.SendToAll(this.GetSongData());
        }

        public override void OnCycle()
        {
            if (this.IsPlaying && this.TimePlaying >= this.GetCurrentSong().GetSoundtrack().Length)
            {
                this.NextSong();
            }
        }

        public ServerMessage GetSongData()
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.SongPlaying);
            if (!this.IsPlaying)
            {
                message.AppendInt32(-1);
                message.AppendInt32(-1);
                message.AppendInt32(-1);
                message.AppendInt32(-1);
                message.AppendInt32(-1);
            }
            else
            {
                MusicDisc disc = this.GetCurrentSong();

                message.AppendInt32(disc.SongID);
                message.AppendInt32(this.SongQueuePosition);
                message.AppendInt32(disc.SongID);
                message.AppendInt32(0);
                message.AppendInt32((int)this.TimePlaying * 1000);
            }
            return message;
        }

        public double TimePlaying
        {
            get
            {
                return (TimeUtilies.GetUnixTimestamp() - this.SongStarted);
            }
        }
    }
}
