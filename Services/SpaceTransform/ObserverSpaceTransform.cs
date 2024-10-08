using DrawPolygons.Models.Primitives;
using DrawPolygons.Models.RenderElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services.SpaceTransform
{
    public class ObserverSpaceTransform
    {
        public VectorFourCoord Eye { get; set; }
        public VectorFourCoord Target { get; set; }
        public VectorFourCoord Up { get; set; }



        public ObserverSpaceTransform(VectorFourCoord eye, VectorFourCoord target, VectorFourCoord up)
        {
            Eye = eye;
            Target = target;
            Up = up;
        }

        private static VectorFourCoord TakeZAxis(VectorFourCoord eye, VectorFourCoord target)
        {
            return eye.SubstractWithVectorWithoutW(target).TakeNormalizedVectorWithoutW();
        }

        private static VectorFourCoord TakeXAxis(VectorFourCoord up, VectorFourCoord zAxis)
        {
            return up.MultiplyWithVectorResultWithoutW(zAxis).TakeNormalizedVectorWithoutW();
        }

        private static VectorFourCoord TakeYAxis(VectorFourCoord up)
        {
            return up;
        }

        private static MatrixFourByFour TakeViewMatrix(VectorFourCoord eye, VectorFourCoord target, VectorFourCoord up)
        {
            VectorFourCoord zAxis = TakeZAxis(eye, target);
            VectorFourCoord xAxis = TakeXAxis(up, zAxis);
            VectorFourCoord yAxis = TakeYAxis(up);

            return new MatrixFourByFour(new float[4, 4]
            {
                { xAxis.X, xAxis.Y, xAxis.Z, -xAxis.MultiplyWithScalarResultWithoutW(eye)},
                { yAxis.X, yAxis.Y, yAxis.Z, -yAxis.MultiplyWithScalarResultWithoutW(eye)},
                { zAxis.X, zAxis.Y, zAxis.Z, -zAxis.MultiplyWithScalarResultWithoutW(eye)},
                { 0,       0,       0,       1                                           }
            });
        }

        public MatrixFourByFour TakeViewMatrix()
        {
            return TakeViewMatrix(Eye, Target, Up);
        }

        public VectorFourCoord TakeTransformVectorToObserverSpace(VectorFourCoord vector)
        {
            return TakeViewMatrix().Myltiply(vector);
        }
    }
}
