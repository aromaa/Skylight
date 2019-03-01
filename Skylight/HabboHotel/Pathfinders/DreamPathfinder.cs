using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Rooms.Bots;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Pathfinders
{
    public class DreamPathfinder
    {
        public static Point[] Directions = { new Point(0, 1), new Point(1, 0), new Point(0, -1), new Point(-1, 0), new Point(1, 1), new Point(-1, -1), new Point(1, -1), new Point(-1, 1) };

        public static RoomTile GetNearestRoomTile(Point3D currentPoint, Point targetPoint, Room room, RoomUnit unit, bool calcDiagonal, bool walktrought)
        {
            double distance = DreamPathfinder.GetDistance(currentPoint.XY, targetPoint);

            RoomTile to = null;
            RoomTile from = unit.CurrentTile;
            for (int i = 0; i < (calcDiagonal ? 8 : 4); i++)
            {
                Point direction = DreamPathfinder.Directions[i];

                RoomTile testTile = room.RoomGamemapManager.GetTile(currentPoint.X + direction.X, currentPoint.Y + direction.Y);
                RoomUnitUser user = unit as RoomUnitUser;
                if (testTile != null && ((user?.Override ?? false) || ((testTile.GetZ(true) - currentPoint.Y) <= 2.0)))
                {
                    bool targetTile = targetPoint.X == testTile.X && targetPoint.Y == testTile.Y;
                    if (!targetTile && (testTile.IsBed || testTile.IsSeat))
                    {
                        continue;
                    }

                    if (testTile.IsInUse && (targetTile || !walktrought)) //user is in the tile
                    {
                        continue;
                    }

                    if (!user?.Override ?? false)
                    {
                        if (testTile.HigestRoomItem is RoomItemHorseObstacle obstacle)
                        {
                            RoomItemHorseObstacle lastObstacle = from.HigestRoomItem as RoomItemHorseObstacle;
                            if (lastObstacle == null || lastObstacle.ID != obstacle.ID || i == 0 || i == 2 || i == 4 || i == 6)
                            {
                                if (obstacle.IsMiddlePart(testTile.X, testTile.Y)) //trying to enter middle part
                                {
                                    if (lastObstacle == null || lastObstacle.ID != obstacle.ID || lastObstacle.IsMiddlePart(from.X, from.Y))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (!(testTile.X == targetPoint.X && testTile.Y == targetPoint.Y)) //middle ISINT the target tile
                                        {
                                            if (unit is RoomPet pet && pet.Rider != null)
                                            {
                                                if (pet.PetData.Type != 13 || pet.PetData.Energy < 3)
                                                {
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (testTile.HigestRoomItem is RoomItemSkateboardRail)
                        {
                            if (user?.ActiveEffect != 71)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (testTile.ModelItemState == ModelItemState.LOCKED || testTile.IsHole || testTile.ItemsOnTile.Get(typeof(RoomItemBlackHole)).Count > 0) //item logic blocking to move to that tile skip the tile
                            {
                                continue;
                            }
                        }
                    }

                    double tileDistance = DreamPathfinder.GetDistance(testTile.X, testTile.Y, targetPoint.X, targetPoint.Y);
                    if (distance > tileDistance)
                    {
                        distance = tileDistance;
                        to = testTile;
                    }
                }
            }

            return to ?? from;
        }

        internal static double GetDistance(Point currentPoint, Point targetPoint)
        {
            return Math.Sqrt(Math.Pow((double)(currentPoint.X - targetPoint.X), 2.0) + Math.Pow((double)(currentPoint.Y - targetPoint.Y), 2.0));
        }

        internal static double GetDistance(int curX, int curY, int tarX, int tarY)
        {
            return Math.Sqrt(Math.Pow((double)(curX - tarX), 2.0) + Math.Pow((double)(curY - tarY), 2.0));
        }
    }
}
