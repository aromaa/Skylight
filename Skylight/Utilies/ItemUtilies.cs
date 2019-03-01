using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class ItemUtilies
    {
        public static Dictionary<int, AffectedTile> AffectedTiles(int lenght, int weight, int x, int y, int rot)
        {
            int num = 0;
            Dictionary<int, AffectedTile> dictionary = new Dictionary<int, AffectedTile>();
            if (lenght > 1)
            {
                if (rot == 0 || rot == 4)
                {
                    for (int i = 1; i < lenght; i++)
                    {
                        dictionary.Add(num++, new AffectedTile(x, y + i, i));
                        for (int j = 1; j < weight; j++)
                        {
                            dictionary.Add(num++, new AffectedTile(x + j, y + i, (i < j) ? j : i));
                        }
                    }
                }
                else
                {
                    if (rot == 2 || rot == 6)
                    {
                        for (int i = 1; i < lenght; i++)
                        {
                            dictionary.Add(num++, new AffectedTile(x + i, y, i));
                            for (int j = 1; j < weight; j++)
                            {
                                dictionary.Add(num++, new AffectedTile(x + i, y + j, (i < j) ? j : i));
                            }
                        }
                    }
                }
            }
            if (weight > 1)
            {
                if (rot == 0 || rot == 4)
                {
                    for (int i = 1; i < weight; i++)
                    {
                        dictionary.Add(num++, new AffectedTile(x + i, y, i));
                        for (int j = 1; j < lenght; j++)
                        {
                            dictionary.Add(num++, new AffectedTile(x + i, y + j, (i < j) ? j : i));
                        }
                    }
                }
                else
                {
                    if (rot == 2 || rot == 6)
                    {
                        for (int i = 1; i < weight; i++)
                        {
                            dictionary.Add(num++, new AffectedTile(x, y + i, i));
                            for (int j = 1; j < lenght; j++)
                            {
                                dictionary.Add(num++, new AffectedTile(x + j, y + i, (i < j) ? j : i));
                            }
                        }
                    }
                }
            }
            return dictionary;
        }
    }
}
