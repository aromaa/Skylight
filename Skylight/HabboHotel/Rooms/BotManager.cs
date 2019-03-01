using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms.Bots;
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
    public class BotManager
    {
        public Dictionary<int, RoomBotData> Bots;
        public List<BotAction> NewbieBotActions;
        public BotManager()
        {
            this.Bots = new Dictionary<int,RoomBotData>();
            this.NewbieBotActions = new List<BotAction>();
        }

        public void LoadBots(DatabaseClient dbClient)
        {
            Logging.Write("Loading bots... ");
            Dictionary<int, RoomBotData> newBots = new Dictionary<int, RoomBotData>();
            List<BotResponse> newBotResponses = new List<BotResponse>();
            List<BotSpeech> newBotSpeech = new List<BotSpeech>();

            DataTable botResponses = dbClient.ReadDataTable("SELECT * FROM bots_responses");
            if (botResponses != null && botResponses.Rows.Count > 0)
            {
                foreach(DataRow dataRow in botResponses.Rows)
                {
                    newBotResponses.Add(new BotResponse((int)dataRow["bot_id"], (string)dataRow["keywords"], (string)dataRow["response_text"], (string)dataRow["mode"] == "say" ? 0 : (string)dataRow["mode"] == "shout" ? 1 : 2, (int)dataRow["serve_id"]));
                }
            }

            DataTable botSpeech = dbClient.ReadDataTable("SELECT * FROM bots_speech");
            if (botSpeech != null && botSpeech.Rows.Count > 0)
            {
                foreach (DataRow dataRow in botSpeech.Rows)
                {
                    newBotSpeech.Add(new BotSpeech((int)dataRow["bot_id"], (string)dataRow["text"], TextUtilies.StringToBool((string)dataRow["shout"])));
                }
            }

            DataTable bots = dbClient.ReadDataTable("SELECT * FROM bots");
            if (bots != null && bots.Rows.Count > 0)
            {
                foreach(DataRow dataRow in bots.Rows)
                {
                    int id = (int)dataRow["id"];
                    newBots.Add(id, new RoomBotData(id, (uint)dataRow["room_id"], (string)dataRow["name"], (string)dataRow["look"], (string)dataRow["motto"], (int)dataRow["x"], (int)dataRow["y"], (double)dataRow["z"], (int)dataRow["rotation"], (string)dataRow["walk_mode"] == "stand" ? 0 : (string)dataRow["walk_mode"] == "freeroam" ? 1 : 2, (int)dataRow["min_x"], (int)dataRow["min_y"], (int)dataRow["max_x"], (int)dataRow["max_y"], (int)dataRow["effect"], newBotResponses.Where(r => r.BotID == id).ToList(), newBotSpeech.Where(s => s.BotID == id).ToList()));
                }
            }

            this.Bots = newBots;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadNewbieBotActions(DatabaseClient dbClient)
        {
            Logging.Write("Loading newbie bot actions... ");
            List<BotAction> newActions = new List<BotAction>();

            DataTable actions = dbClient.ReadDataTable("SELECT action, value, tick FROM bots_newbie_bot_actions");
            if (actions != null && actions.Rows.Count > 0)
            {
                foreach(DataRow dataRow in actions.Rows)
                {
                    newActions.Add(new BotAction((string)dataRow["action"], (string)dataRow["value"], (int)dataRow["tick"]));
                }
            }

            this.NewbieBotActions = newActions;
            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public List<BotAction> GetBotActionsByTick(int tick)
        {
            return this.NewbieBotActions.Where(a => a.Tick == tick).ToList();
        }

        public List<RoomBotData> GetBotsByRoomId(uint roomId)
        {
            return this.Bots.Values.Where(b => b.RoomID == roomId).ToList();
        }
    }
}
