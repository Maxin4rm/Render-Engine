using DrawPolygons.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services.SpaceTransform
{
    public class ProjectionSpaceTransform
    {
        public float CameraHeight { get; set; }
        public float CameraWidth { get; set; }
        public float FovRadians { get; set; }
        public float ZNear { get; set; }
        public float ZFar { get; set; }

        public ProjectionSpaceTransform(float cameraHeight, float cameraWidth, float fovRadians, float zNear, float zFar)
        {
            this.CameraHeight = cameraHeight;
            this.CameraWidth = cameraWidth;
            this.FovRadians = fovRadians;
            this.ZNear = zNear;
            this.ZFar = zFar;
        }

        public static MatrixFourByFour TakeMatrixProjection(float cameraHeight, float cameraWidth, float fovRadians, float zNear, float zFar)
        {
            float aspect = cameraWidth / cameraHeight;

            return new MatrixFourByFour(new float[4, 4]
            {
                { 1 / (aspect * (float) Math.Tan(fovRadians / 2)), 0,                                      0,                     0                            },
                { 0,                                               1 / (float) Math.Tan(fovRadians / 2),   0,                     0                            },
                { 0,                                               0,                                      zFar / (zNear - zFar), zNear * zFar / (zNear - zFar)},
                { 0,                                               0,                                      -1,                    0                            },
            });
        }

        public MatrixFourByFour TakeMatrixProjection()
        {
            return TakeMatrixProjection(CameraHeight, CameraWidth, FovRadians, ZNear, ZFar);
        }

        public VectorFourCoord? TakeTransformVectorToProjectionSpace(VectorFourCoord vector)
        {
            VectorFourCoord vectorTransformed = TakeMatrixProjection().Myltiply(vector);

            if (vectorTransformed.W <= 0)
            {
                return null;
            }
            else
            {
                return new VectorFourCoord(vectorTransformed.X / vectorTransformed.W, vectorTransformed.Y / vectorTransformed.W, vectorTransformed.Z / vectorTransformed.W, vectorTransformed.W / vectorTransformed.W);
            }
        }
    }
}
