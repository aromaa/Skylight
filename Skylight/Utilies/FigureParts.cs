using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class FigureParts
    {
        private Dictionary<string, Dictionary<string, object>> Parts;
        private string Figure;

        public FigureParts(string figure)
        {
            this.Parts = new Dictionary<string, Dictionary<string, object>>();
            this.Figure = figure;

            this.ParseParts();
        }

        private void ParseParts()
        {
            foreach(string part in this.Figure.Split('.'))
            {
                string[] partData = part.Split('-');
                if (partData.Length >= 2)
                {
                    string type = partData[0];
                    string setId = partData[1];

                    List<string> colorIds = new List<string>();
                    for(int i = 2; i < partData.Length; i++)
                    {
                        colorIds.Add(partData[i]);
                    }
                    this.AddPart(type, setId, colorIds);
                }
            }
        }

        private void AddPart(string type, string setId, List<string> colors)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("type", type);
            data.Add("setid", setId);
            data.Add("colorids", colors);
            this.Parts.Add(type, data);
        }

        public Dictionary<string, Dictionary<string, object>> GetParts()
        {
            return this.Parts;
        }

        public Dictionary<string, object> GetPart(string type)
        {
            Dictionary<string, object> data;
            this.Parts.TryGetValue(type, out data);
            return data;
        }

        public void ReplacePart(Dictionary<string, object> data)
        {
            if (this.Parts.ContainsKey(data["type"] as string))
            {
                this.Parts[data["type"] as string] = data;
            }
            else
            {
                this.Parts.Add(data["type"] as string, data);
            }
        }

        public string GetPartString()
        {
            string figure = "";
            foreach(KeyValuePair<string, Dictionary<string, object>> part in this.Parts)
            {
                if (figure.Length > 0)
                {
                    figure += ".";
                }

                figure += part.Key + "-" + part.Value["setid"];
                List<string> colors = part.Value["colorids"] as List<string>;
                if (colors != null)
                {
                    foreach(string color in colors)
                    {
                        figure += "-" + color;
                    }
                }
            }
            return figure;
        }
    }
}
