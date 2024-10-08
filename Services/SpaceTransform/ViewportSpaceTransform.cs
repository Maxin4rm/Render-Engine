using DrawPolygons.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services.SpaceTransform
{
    public class ViewportSpaceTransform
    {
        public float WindowWidth { get; set; }
        public float WindowHeight { get; set; }
        public float XMin { get; set; }
        public float YMin { get; set; }

        public ViewportSpaceTransform(float windowWidth, float windowHeight, float xMin, float yMin)
        {
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
            this.XMin = xMin;
            this.YMin = yMin;
        }

        public static MatrixFourByFour TakeMatrixViewport(float windowWidth, float windowHeight, float xMin, float yMin)
        {
            return new MatrixFourByFour(new float[4, 4]
            {
                { windowWidth / 2, 0,                 0, xMin + windowWidth / 2 },
                { 0,               -windowHeight / 2, 0, yMin + windowHeight / 2},
                { 0,               0,                 1, 0                      },
                { 0,               0,                 0, 1                      },
            });
        }

        public MatrixFourByFour TakeMatrixViewport()
        {
            return TakeMatrixViewport(WindowWidth, WindowHeight, XMin, YMin);
        }

        public VectorFourCoord TakeTransformVectorToViewportSpace(VectorFourCoord vector)
        {
            return TakeMatrixViewport().Myltiply(vector);
        }
    }
}
