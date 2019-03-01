using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public struct Point3D
    {
        private Point xy;
        public Point XY
        {
            get
            {
                return this.xy;
            }
            set
            {
                this.xy = value;
            }
        }

        public int X
        {
            get
            {
                return this.xy.X;
            }
            set
            {
                this.xy.X = value;
            }
        }
        public int Y
        {
            get
            {
                return this.xy.Y;
            }
            set
            {
                this.xy.Y = value;
            }
        }
        public double Z { get; set; }

        public Point3D(int x, int y, double z)
        {
            this.xy = new Point(x, y);
            this.Z = z;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point3D other && other == this)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(Point3D left, Point3D right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Point3D left, Point3D right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
    }
}
