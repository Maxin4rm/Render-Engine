using DrawPolygons.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services
{
    public static class RejectingInvisibleTriangles
    {
        public static bool IsVisibleTriangle(VectorFourCoord vectorTriangleNormal, VectorFourCoord vectorEye, VectorFourCoord vectorTriangleFirst)
        {
            VectorFourCoord vectorLook = new VectorFourCoord(vectorEye.X - vectorTriangleFirst.X, vectorEye.Y - vectorTriangleFirst.Y, vectorEye.Z - vectorTriangleFirst.Z);
            vectorLook.TakeNormalizedVectorWithoutW();
                      
            return vectorTriangleNormal.MultiplyWithScalarResultWithoutW(vectorLook) > 0.0f;
        }
    }
}
