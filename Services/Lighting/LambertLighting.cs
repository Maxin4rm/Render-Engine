using DrawPolygons.Models.Primitives;
using DrawPolygons.Models.RenderElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services.Lighting
{
    public static class LambertLighting
    {
        public static Color TakeLightIntensityForTriangle(VectorFourCoord vectorLightDirection, VectorFourCoord vectorTriangleNormal, Color colorTriangle)
        {           
            float angleCosRadians = Math.Max(vectorLightDirection.MultiplyWithScalarResultWithoutW(vectorTriangleNormal), 0);
            
            return Color.FromArgb((int)(angleCosRadians * colorTriangle.R), (int)(angleCosRadians * colorTriangle.G), (int)(angleCosRadians * colorTriangle.B));
        }   
    }
}
