using SkylightEmulator.HabboHotel.Rooms;
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
        public static RoomTile GetNearestRoomTile(Point currentPoint, Point targetPoint, double height, Room room, bool calcDiagonal)
        {
            double distance = GetDistance(currentPoint, targetPoint);

            Point[] possiblePoints = new Point[8];
            if (calcDiagonal)
            {
                possiblePoints[1] = new Point(currentPoint.X - 1, currentPoint.Y - 1);
                possiblePoints[3] = new Point(currentPoint.X - 1, currentPoint.Y + 1);
                possiblePoints[5] = new Point(currentPoint.X + 1, currentPoint.Y + 1);
                possiblePoints[7] = new Point(currentPoint.X + 1, currentPoint.Y - 1);
            }
            possiblePoints[0] = new Point(currentPoint.X, currentPoint.Y - 1);
            possiblePoints[2] = new Point(currentPoint.X - 1, currentPoint.Y);
            possiblePoints[4] = new Point(currentPoint.X, currentPoint.Y + 1);
            possiblePoints[6] = new Point(currentPoint.X + 1, currentPoint.Y);

            RoomTile bestTile = room.RoomGamemapManager.GetTile(currentPoint.X, currentPoint.Y);
            for (int i = 0; i < 8; i++)
            {
                Point testPoint = possiblePoints[i];
                if (testPoint != null)
                {
                    RoomTile testTile = room.RoomGamemapManager.GetTile(testPoint.X, testPoint.Y);
                    if (testTile != null && testTile.CanUserMoveToTile && (testTile.GetZ(true) - height) <= 2.0)
                    {
                        if ((testTile.IsBed || testTile.IsSeat) && testPoint != targetPoint)
                        {
                            continue;
                        }

                        double tileDistance = GetDistance(testPoint, targetPoint);
                        if (distance > tileDistance)
                        {
                            distance = tileDistance;
                            bestTile = testTile;
                        }
                    }
                }
            }

            return bestTile;
        }

        internal static double GetDistance(Point currentPoint, Point targetPoint)
        {
            return Math.Sqrt(Math.Pow((double)(currentPoint.X - targetPoint.X), 2.0) + Math.Pow((double)(currentPoint.Y - targetPoint.Y), 2.0));
        }
    }
}
