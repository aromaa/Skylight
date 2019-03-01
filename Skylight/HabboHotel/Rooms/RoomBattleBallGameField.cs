using SkylightEmulator.HabboHotel.Rooms.Algorithm;
using SkylightEmulator.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomBattleBallGameField : IPathNode
    {
        private GameTeam[,] currentField;
        private Queue<RoomBattleBanzaiTileStateUpdate> newEntries = new Queue<RoomBattleBanzaiTileStateUpdate>();
        private RoomBattleBanzaiTileStateUpdate currentlyChecking;
        private AStarSolver<RoomBattleBallGameField> astarSolver;

        public RoomBattleBallGameField(GameTeam[,] theArray)
        {
            this.currentField = theArray;
            this.astarSolver = new AStarSolver<RoomBattleBallGameField>(true, AStarHeuristicType.EXPERIMENTAL_SEARCH, this, theArray.GetUpperBound(1) + 1, theArray.GetUpperBound(0) + 1);
        }

        public void updateLocation(int x, int y, GameTeam value)
        {
            this.newEntries.Enqueue(new RoomBattleBanzaiTileStateUpdate(x, y, value));
        }

        public List<PointField> doUpdate(bool oneloop = false)
        {
            List<PointField> returnList = new List<PointField>();
            while (this.newEntries.Count > 0)
            {
                this.currentlyChecking = this.newEntries.Dequeue();

                List<System.Drawing.Point> pointList = getConnectedItems(this.currentlyChecking);
                if (pointList.Count > 1)
                {
                    List<LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode>> RouteList = handleListOfConnectedPoints(pointList, this.currentlyChecking);
                    foreach (LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode> nodeList in RouteList)
                    {
                        if (nodeList.Count >= 1)
                        {
                            PointField field = findClosed(nodeList);
                            if (field != null)
                            {
                                returnList.Add(field);
                            }
                        }
                    }
                }
                this.currentField[this.currentlyChecking.Y, this.currentlyChecking.X] = this.currentlyChecking.GameTeam;
            }
            return returnList;
        }

        private List<System.Drawing.Point> getConnectedItems(RoomBattleBanzaiTileStateUpdate update)
        {
            List<System.Drawing.Point> ConnectedItems = new List<System.Drawing.Point>();
            int x = update.X;
            int y = update.Y;
            if (true)
            {
                if (this[y - 1, x - 1] && this.currentField[y - 1, x - 1] == update.GameTeam)
                {
                    ConnectedItems.Add(new System.Drawing.Point(x - 1, y - 1));
                }
                if (this[y - 1, x + 1] && this.currentField[y - 1, x + 1] == update.GameTeam)
                {
                    ConnectedItems.Add(new System.Drawing.Point(x + 1, y - 1));
                }
                if (this[y + 1, x - 1] && this.currentField[y + 1, x - 1] == update.GameTeam)
                {
                    ConnectedItems.Add(new System.Drawing.Point(x - 1, y + 1));
                }
                if (this[y + 1, x + 1] && this.currentField[y + 1, x + 1] == update.GameTeam)
                {
                    ConnectedItems.Add(new System.Drawing.Point(x + 1, y + 1));
                }
            }


            if (this[y - 1, x] && this.currentField[y - 1, x] == update.GameTeam)
            {
                ConnectedItems.Add(new System.Drawing.Point(x, y - 1));
            }
            if (this[y + 1, x] && this.currentField[y + 1, x] == update.GameTeam)
            {
                ConnectedItems.Add(new System.Drawing.Point(x, y + 1));
            }
            if (this[y, x - 1] && this.currentField[y, x - 1] == update.GameTeam)
            {
                ConnectedItems.Add(new System.Drawing.Point(x - 1, y));
            }
            if (this[y, x + 1] && this.currentField[y, x + 1] == update.GameTeam)
            {
                ConnectedItems.Add(new System.Drawing.Point(x + 1, y));
            }

            return ConnectedItems;
        }

        private List<LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode>> handleListOfConnectedPoints(List<System.Drawing.Point> pointList, RoomBattleBanzaiTileStateUpdate update)
        {
            List<LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode>> returnList = new List<LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode>>();
            int amount = 0;
            foreach (System.Drawing.Point begin in pointList)
            {
                amount++;
                if (amount == pointList.Count / 2 + 1)
                    return returnList;
                foreach (System.Drawing.Point end in pointList)
                {
                    if (begin == end)
                        continue;
                    LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode> list = this.astarSolver.Search(end, begin);
                    if (list != null)
                    {
                        returnList.Add(list);
                    }
                }
            }
            return returnList;
        }


        public bool this[int y, int x]
        {
            get
            {
                if (y < 0 || x < 0)
                    return false;
                else if (y > this.currentField.GetUpperBound(0) || x > this.currentField.GetUpperBound(1))
                    return false;
                return true;
            }

        }

        private PointField findClosed(LinkedList<AStarSolver<RoomBattleBallGameField>.PathNode> nodeList)
        {
            PointField returnList = new PointField(this.currentlyChecking.GameTeam);

            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach (AStarSolver<RoomBattleBallGameField>.PathNode node in nodeList)
            {
                if (node.X < minX)
                    minX = node.X;

                if (node.X > maxX)
                    maxX = node.X;

                if (node.Y < minY)
                    minY = node.Y;

                if (node.Y > maxY)
                    maxY = node.Y;

            }

            int middleX = (int)Math.Ceiling(((maxX - minX) / 2f)) + minX;
            int middleY = (int)Math.Ceiling(((maxY - minY) / 2f)) + minY;
            //Console.WriteLine("Middle: x:[{0}]  y:[{1}]", middleX, middleY);

            System.Drawing.Point current;
            List<System.Drawing.Point> toFill = new List<System.Drawing.Point>();
            List<System.Drawing.Point> checkedItems = new List<System.Drawing.Point>();
            checkedItems.Add(new System.Drawing.Point(this.currentlyChecking.X, this.currentlyChecking.Y));
            System.Drawing.Point toAdd;
            toFill.Add(new System.Drawing.Point(middleX, middleY));
            int x;
            int y;
            while (toFill.Count > 0)
            {
                current = toFill[0];
                x = current.X;
                y = current.Y;

                if (x < minX)
                    return null;//OOB
                if (x > maxX)
                    return null;//OOB
                if (y < minY)
                    return null;//OOB
                if (y > maxY)
                    return null; //OOB

                if (this[y - 1, x] && this.currentField[y - 1, x] == 0)
                {
                    toAdd = new System.Drawing.Point(x, y - 1);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (this[y + 1, x] && this.currentField[y + 1, x] == 0)
                {
                    toAdd = new System.Drawing.Point(x, y + 1);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (this[y, x - 1] && this.currentField[y, x - 1] == 0)
                {
                    toAdd = new System.Drawing.Point(x - 1, y);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (this[y, x + 1] && this.currentField[y, x + 1] == 0)
                {
                    toAdd = new System.Drawing.Point(x + 1, y);
                    if (!toFill.Contains(toAdd) && !checkedItems.Contains(toAdd))
                        toFill.Add(toAdd);
                }
                if (getValue(current) == 0)
                    returnList.Add(current);
                checkedItems.Add(current);
                toFill.RemoveAt(0);


            }

            return returnList;
        }

        public GameTeam getValue(int x, int y)
        {
            if (this[y, x])
            {
                return this.currentField[y, x];
            }
            return GameTeam.None;
        }

        public GameTeam getValue(System.Drawing.Point p)
        {
            if (this[p.Y, p.X])
            {
                return this.currentField[p.Y, p.X];
            }
            return GameTeam.None;
        }

        public bool IsBlocked(int x, int y, bool lastTile)
        {
            if (this.currentlyChecking.X == x && this.currentlyChecking.Y == y)
                return true;
            return !(getValue(x, y) == this.currentlyChecking.GameTeam);
        }

        public void destroy()
        {
            this.currentField = null;
        }
    }
}
