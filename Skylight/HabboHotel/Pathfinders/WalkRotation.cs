using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Pathfinders
{
    public class WalkRotation
    {
        public static int Walk(int x, int y, int targetX, int targetY)
        {
            int result = 0;
            if (x > targetX && y > targetY)
            {
                result = 7;
            }
            else
            {
                if (x < targetX && y < targetY)
                {
                    result = 3;
                }
                else
                {
                    if (x > targetX && y < targetY)
                    {
                        result = 5;
                    }
                    else
                    {
                        if (x < targetX && y > targetY)
                        {
                            result = 1;
                        }
                        else
                        {
                            if (x > targetX)
                            {
                                result = 6;
                            }
                            else
                            {
                                if (x < targetX)
                                {
                                    result = 2;
                                }
                                else
                                {
                                    if (y < targetY)
                                    {
                                        result = 4;
                                    }
                                    else
                                    {
                                        if (y > targetY)
                                        {
                                            result = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static int Moonwalk(int x, int y, int targetX, int targetY)
        {
            int result = 0;
            if (x > targetX && y > targetY)
            {
                result = 3;
            }
            else
            {
                if (x < targetX && y < targetY)
                {
                    result = 7;
                }
                else
                {
                    if (x > targetX && y < targetY)
                    {
                        result = 1;
                    }
                    else
                    {
                        if (x < targetX && y > targetY)
                        {
                            result = 5;
                        }
                        else
                        {
                            if (x > targetX)
                            {
                                result = 2;
                            }
                            else
                            {
                                if (x < targetX)
                                {
                                    result = 6;
                                }
                                else
                                {
                                    if (y < targetY)
                                    {
                                        result = 0;
                                    }
                                    else
                                    {
                                        if (y > targetY)
                                        {
                                            result = 4;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
