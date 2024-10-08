using DrawPolygons.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Models.RenderElements
{
    public class LightSource
    {
        public VectorFourCoord VectorLight { get; set; }
        public Color Color { get; set; }

        public LightSource(VectorFourCoord vectorLight, Color color)
        {
            this.VectorLight = vectorLight;
            this.Color = color;
        }
    }
}
