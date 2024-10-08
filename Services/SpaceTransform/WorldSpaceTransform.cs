using DrawPolygons.Models.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Services.SpaceTransform
{
    public class WorldSpaceTransform
    {
        public VectorFourCoord VectorTranslation { get; set; }
        public VectorFourCoord VectorScale { get; set; }
        public float AngleRadiansX { get; set; }
        public float AngleRadiansY { get; set; }
        public float AngleRadiansZ { get; set; }

        public WorldSpaceTransform(VectorFourCoord vectorTranslation, VectorFourCoord vectorScale, float angleRadiansX, float angleRadiansY, float angleRadiansZ)
        {
            if (vectorTranslation.W != 1)
            {
                throw new Exception("Vector of translation should have coordinate \"w\" = 1!");
            }

            VectorTranslation = vectorTranslation;
            VectorScale = vectorScale;
            AngleRadiansX = angleRadiansX;
            AngleRadiansY = angleRadiansY;
            AngleRadiansZ = angleRadiansZ;
        }

        private static MatrixFourByFour TakeMatrixTranslation(VectorFourCoord vectorTranslation)
        {
            if (vectorTranslation.W != 1)
            {
                throw new Exception("Vector of translation should have coordinate \"w\" = 1!");
            }

            return new MatrixFourByFour(new float[4, 4] {
                { 1, 0, 0, vectorTranslation.X},
                { 0, 1, 0, vectorTranslation.Y},
                { 0, 0, 1, vectorTranslation.Z},
                { 0, 0, 0, vectorTranslation.W},
            });
        }

        private static MatrixFourByFour TakeMatrixScale(VectorFourCoord vectorScale)
        {
            return new MatrixFourByFour(new float[4, 4] {
                { vectorScale.X, 0,             0,             0},
                { 0,             vectorScale.Y, 0,             0},
                { 0,             0,             vectorScale.Z, 0},
                { 0,             0,             0,             1},
            });
        }

        private static MatrixFourByFour TakeMatrixRotateX(float angleRadiansX)
        {
            return new MatrixFourByFour(new float[4, 4] {
                { 1,             0,                               0,                                0},
                { 0,             (float) Math.Cos(angleRadiansX), -(float) Math.Sin(angleRadiansX), 0},
                { 0,             (float) Math.Sin(angleRadiansX), (float) Math.Cos(angleRadiansX),  0},
                { 0,             0,                               0,                                1},
            });
        }

        private static MatrixFourByFour TakeMatrixRotateY(float angleRadiansY)
        {
            return new MatrixFourByFour(new float[4, 4] {
                { (float) Math.Cos(angleRadiansY),  0, (float) Math.Sin(angleRadiansY), 0},
                { 0,                                1, 0,                               0},
                { -(float) Math.Sin(angleRadiansY), 0, (float) Math.Cos(angleRadiansY), 0},
                { 0,                                0, 0,                               1},
            });
        }

        private static MatrixFourByFour TakeMatrixRotateZ(float angleRadiansZ)
        {
            return  new MatrixFourByFour(new float[4, 4] {
                { (float) Math.Cos(angleRadiansZ), -(float) Math.Sin(angleRadiansZ), 0, 0},
                { (float) Math.Sin(angleRadiansZ), (float) Math.Cos(angleRadiansZ),  0, 0},
                { 0,                               0,                                1, 0},
                { 0,                               0,                                0, 1},
            });
        }

        public static MatrixFourByFour TakeMatrixModel(VectorFourCoord vectorTranslation, VectorFourCoord vectorScale, float angleRadiansX, float angleRadiansY, float angleRadiansZ)
        {
            return TakeMatrixTranslation(vectorTranslation).
                Myltiply(TakeMatrixRotateZ(angleRadiansZ)).
                Myltiply(TakeMatrixRotateY(angleRadiansY)).
                Myltiply(TakeMatrixRotateX(angleRadiansX)).
                Myltiply(TakeMatrixScale(vectorScale));
        }

        public MatrixFourByFour TakeMatrixTranslation()
        {
            return TakeMatrixTranslation(VectorTranslation);
        }

        public MatrixFourByFour TakeMatrixRotateZ()
        {
            return TakeMatrixRotateZ(AngleRadiansZ);
        }

        public MatrixFourByFour TakeMatrixRotateY()
        {
            return TakeMatrixRotateY(AngleRadiansY);
        }

        public MatrixFourByFour TakeMatrixRotateX()
        {
            return TakeMatrixRotateX(AngleRadiansX);
        }

        public MatrixFourByFour TakeMatrixScale()
        {
            return TakeMatrixScale(VectorScale);
        }

        public MatrixFourByFour TakeMatrixModel()
        {
            return TakeMatrixModel(VectorTranslation, VectorScale, AngleRadiansX, AngleRadiansY, AngleRadiansZ);
        }

        public VectorFourCoord TakeTransformVectorToWorldSpace(VectorFourCoord vector)
        {
            return TakeMatrixModel().Myltiply(vector);
        }
    }
}
