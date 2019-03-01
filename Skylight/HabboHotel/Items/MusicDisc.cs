using SkylightEmulator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items
{
    public class MusicDisc
    {
        public uint ItemId;
        public uint BaseItem;
        public int SongID;

        public MusicDisc(uint itemId, uint baseItem, int songId)
        {
            this.ItemId = itemId;
            this.BaseItem = baseItem;
            this.SongID = songId;
        }

        public Soundtrack GetSoundtrack()
        {
            return Skylight.GetGame().GetItemManager().TryGetSoundtrack(this.SongID);
        }
    }
}
