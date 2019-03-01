using SkylightEmulator.Core;
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
    public class MoodlightData
    {
        public readonly uint ItemID;
        public int CurrentPreset;
        public List<MoodlightPreset> Presets;
        public bool Enabled;

        public MoodlightData(uint itemId)
        {
            this.ItemID = itemId;
            this.Presets = new List<MoodlightPreset>();

            DataRow dataRow = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("itemId", this.ItemID);
                dataRow = dbClient.ReadDataRow("SELECT * FROM items_moodlight WHERE item_id = @itemId LIMIT 1");
            }

            this.Enabled = TextUtilies.StringToBool((string)dataRow["enabled"]);
            this.CurrentPreset = (int)dataRow["current_preset"];
            this.Presets.Add(this.StringToMoodlightPreset((string)dataRow["preset_one"]));
            this.Presets.Add(this.StringToMoodlightPreset((string)dataRow["preset_two"]));
            this.Presets.Add(this.StringToMoodlightPreset((string)dataRow["preset_three"]));
        }

        public MoodlightPreset StringToMoodlightPreset(string data)
        {
            string[] dataSplitted = data.Split(',');
            if (!this.IsValidColor(dataSplitted[0]))
            {
                dataSplitted[0] = "#000000";
            }
            return new MoodlightPreset(dataSplitted[0], int.Parse(dataSplitted[1]), TextUtilies.StringToBool(dataSplitted[2]));
        }

        public bool IsValidColor(string color)
        {
            switch (color)
            {
                case "#000000":
                case "#0053F7":
                case "#EA4532":
                case "#82F349":
                case "#74F5F5":
                case "#E759DE":
                case "#F2F851":
                    return true;
                default:
                    return false;
            }
        }

        public void Disable()
        {
            this.Enabled = false;

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("itemId", this.ItemID);
                dbClient.ExecuteQuery("UPDATE items_moodlight SET enabled = '0' WHERE item_id = @itemId LIMIT 1");
            }
        }

        public void Enable()
        {
            this.Enabled = true;

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("itemId", this.ItemID);
                dbClient.ExecuteQuery("UPDATE items_moodlight SET enabled = '1' WHERE item_id = @itemId LIMIT 1");
            }
        }

        public MoodlightPreset GetCurrentPreset()
        {
            return this.Presets[this.CurrentPreset - 1];
        }

        public MoodlightPreset GetPreset(int id)
        {
            return this.Presets[id - 1];
        }

        public string GenerateExtraData()
        {
            MoodlightPreset preset = this.GetCurrentPreset();
            return (this.Enabled ? "2" : "1") + "," + this.CurrentPreset + "," + (preset.BackgroundOnly ? "2" : "1") + "," + preset.ColorCode + "," + preset.ColorIntensity;
        }

        public void SetCurrentPresetSettings(int preset, bool backgroundOnly, string colorCode, int colorIntensity)
        {
            if (this.IsValidColor(colorCode) && colorIntensity >= 0 && colorIntensity <= 255)
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("data", colorCode + "," + colorIntensity + "," + TextUtilies.BoolToString(backgroundOnly));
                    dbClient.AddParamWithValue("itemId", this.ItemID);
                    dbClient.ExecuteQuery("UPDATE items_moodlight SET preset_" + (this.CurrentPreset == 1 ? "one" : this.CurrentPreset == 2 ? "two" : "three" ) + " = @data WHERE item_id = @itemId LIMIT 1");
                }

                this.GetPreset(preset).BackgroundOnly = backgroundOnly;
                this.GetPreset(preset).ColorCode = colorCode;
                this.GetPreset(preset).ColorIntensity = colorIntensity;
            }
        }
    }
}
