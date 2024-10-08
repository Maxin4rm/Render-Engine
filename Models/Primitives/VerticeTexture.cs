using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Models.Primitives
{
    public class VerticeTexture
    {
        private const int COORDINATES_VALUE_MIN = 0;
        private const int COORDINATES_VALUE_MAX = 1;
        private const int V_DEFAULT = 0;
        private const int W_DEFAULT = 1;

        public float U { get; set; }
        public float V { get; set; }
        public float W { get; set; }

        public VerticeTexture(float u, float v, float w)
        {
            U = u;
            V = v;
            W = w;
        }

        public VerticeTexture(float u, float v)
        {
            U = u;
            V = v;
            W = W_DEFAULT;
        }

        public VerticeTexture(float u)
        {
            U = u;
            V = V_DEFAULT;
            W = W_DEFAULT;
        }
    }
}
