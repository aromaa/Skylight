using SkylightEmulator.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class PointField
    {
        private static readonly Point BadPoint = new Point(-1, -1);

        private List<Point> PointList;
        private Point MostLeft = PointField.BadPoint;
        private Point MostTop = PointField.BadPoint;
        private Point MostRight = PointField.BadPoint;
        private Point MostDown = PointField.BadPoint;
        public GameTeam GameTeam { get; private set; }

        public PointField(GameTeam gameTeam)
        {
            this.PointList = new List<Point>();
            this.GameTeam = gameTeam;
        }

        public List<Point> GetPoints()
        {
            return this.PointList;
        }

        public void Add(Point p)
        {
            if (this.MostLeft == PointField.BadPoint)
                this.MostLeft = p;
            if (this.MostRight == PointField.BadPoint)
                this.MostRight = p;
            if (this.MostTop == PointField.BadPoint)
                this.MostTop = p;
            if (this.MostDown == PointField.BadPoint)
                this.MostDown = p;

            if (p.X < this.MostLeft.X)
                this.MostLeft = p;
            if (p.X > this.MostRight.X)
                this.MostRight = p;
            if (p.Y > this.MostTop.Y)
                this.MostTop = p;
            if (p.Y < this.MostDown.Y)
                this.MostDown = p;


            this.PointList.Add(p);
        }
    }
}
