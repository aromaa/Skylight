using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomIcon
    {
        public int BackgroundImage;
        public int ForegroundImage;
        public Dictionary<int, int> Items;

        public RoomIcon(int backgroundImage, int foregroundImage, string items)
        {
            this.BackgroundImage = backgroundImage;
            this.ForegroundImage = foregroundImage;
            this.Items = new Dictionary<int, int>();

            if (!string.IsNullOrEmpty(items))
            {
                foreach(string item in items.Split('|'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] itemParts = item.Replace('.', ',').Split(',');
                        if (itemParts.Length >= 2)
                        {
                            int key = 0;
                            int value = 0;
                            if (int.TryParse(itemParts[0], out key) && int.TryParse(itemParts[1], out value))
                            {
                                if (!this.Items.ContainsKey(key))
                                {
                                    this.Items.Add(key, value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public RoomIcon(int backgroundImage, int foregroundImage, Dictionary<int, int> items)
        {
            this.BackgroundImage = backgroundImage;
            this.ForegroundImage = foregroundImage;
            this.Items = items;
        }

        public void Serialize(ServerMessage message)
        {
            message.AppendInt32(this.BackgroundImage);
            message.AppendInt32(this.ForegroundImage);
            message.AppendInt32(this.Items.Count);

            foreach (KeyValuePair<int, int> Item in this.Items)
            {
                message.AppendInt32(Item.Key);
                message.AppendInt32(Item.Value);
            }
        }

        public string ItemsToString()
        {
            string items = "";
            int count = 0;

            foreach (KeyValuePair<int, int> Item in this.Items)
            {
                if (count > 0)
                {
                    items += "|";
                }
                items += Item.Key + "," + Item.Value;

                count++;
            }

            return items;
        }
    }
}
