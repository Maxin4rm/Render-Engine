using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Models.Primitives
{
    public class VerticeGeometric
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public float W { get; }

        public VerticeGeometric(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public VerticeGeometric(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            W = 1;
        }
    }
}
