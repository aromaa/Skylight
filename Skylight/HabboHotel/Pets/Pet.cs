using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Pets
{
    public class Pet
    {
        public uint ID;
        public uint OwnerID;
        public int Type;

        public string Name;
        public string Race;
        public string Color;

        public int Expirience;
        public int Energy;
        public int Happiness;
        public int Respect;

        public double CreatedTimestamp;
        public bool NeedUpdate = false;
        public static readonly int[] ExperienceLevels = new int[] { 100, 200, 400, 600, 900, 1300, 1800, 2400, 3200, 4300, 5700, 7600, 10100, 13300, 17500, 23000, 30200, 39600, 51900 };

        public int Level
        {
            get
            {
                int level = 0;

                for (int i = 0; i < Pet.ExperienceLevels.Length; i++)
                {
                    if (Pet.ExperienceLevels[i] <= this.Expirience)
                    {
                        continue;
                    }
                    else
                    {
                        level = i;
                        break;
                    }
                }

                return level + 1;
            }
        }

        public int MaxLevel
        {
            get
            {
                return Pet.ExperienceLevels.Length + 1;
            }
        }

        public int ExpirienceGoal
        {
            get
            {
                if (this.Level != this.MaxLevel)
                {
                    return Pet.ExperienceLevels[this.Level - 1];
                }
                else
                {
                    return Pet.ExperienceLevels.Last();
                }
            }
        }

        public int MaxEnergy
        {
            get
            {
                return 100 + this.Level * 20;
            }
        }

        public int MaxHappiness
        {
            get
            {
                return 100;
            }
        }

        public string Look
        {
            get
            {
                return this.Type + " " + this.Race + " " + this.Color.ToLower();
            }
        }

        public int Age
        {
            get
            {
                return (int)Math.Floor((TimeUtilies.GetUnixTimestamp() - this.CreatedTimestamp) / 86400.0);
            }
        }

        public int HorseJumpLevel
        {
            get
            {
                return 1;
            }
        }

        public Pet(uint petId, uint ownerId, int type, string name, string race, string color, int expirience, int energy, int happiness, int respect, double createdTimestamp)
        {
            this.ID = petId;
            this.OwnerID = ownerId;
            this.Type = type;
            this.Name = name;
            this.Race = race;
            this.Color = color;
            this.Expirience = expirience;
            this.Energy = energy;
            this.Happiness = happiness;
            this.Respect = respect;
            this.CreatedTimestamp = createdTimestamp;
        }
    }
}
