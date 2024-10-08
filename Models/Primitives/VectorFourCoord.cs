using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Models.Primitives
{
    public class VectorFourCoord
    {
        private const int W_DEFAULT = 1;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public VectorFourCoord(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public VectorFourCoord(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = W_DEFAULT;
        }

        public float CalculateVectorLengthWithoutW()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public float CalculateVectorLength()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public VectorFourCoord TakeNormalizedVectorWithoutW()
        {
            float vectorLength = CalculateVectorLengthWithoutW();

            return new VectorFourCoord(X / vectorLength, Y / vectorLength, Z / vectorLength, W);
        }

        public VectorFourCoord MultiplyWithVectorResultWithoutW(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(Y * vectorOther.Z - Z * vectorOther.Y, Z * vectorOther.X - X * vectorOther.Z, X * vectorOther.Y - Y * vectorOther.X);
        }

        public float MultiplyWithScalarResultWithoutW(VectorFourCoord vectorOther)
        {
            return X * vectorOther.X + Y * vectorOther.Y + Z * vectorOther.Z;
        }

        public VectorFourCoord SubstractWithVectorWithoutW(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(X - vectorOther.X, Y - vectorOther.Y, Z - vectorOther.Z);
        }

        public VectorFourCoord MultiplyWithVectorResultWithoutW(float number)
        {
            return new VectorFourCoord(X * number, Y * number, Z * number, W);
        }

        public VectorFourCoord SumCoordinatesWithoutW(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(X + vectorOther.X, Y + vectorOther.Y, Z + vectorOther.Z, W);
        }

        public VectorFourCoord MultiplyCoordinatesWithoutW(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(X * vectorOther.X, Y * vectorOther.Y, Z * vectorOther.Z, W);
        }

        public VectorFourCoord TakeNormalizedVector()
        {
            float vectorLength = CalculateVectorLength();

            return new VectorFourCoord(X / vectorLength, Y / vectorLength, Z / vectorLength, W / vectorLength);
        }

        public float MultiplyWithScalarResult(VectorFourCoord vectorOther)
        {
            return X * vectorOther.X + Y * vectorOther.Y + Z * vectorOther.Z + W * vectorOther.W;
        }

        public VectorFourCoord SubstractWithVector(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(X - vectorOther.X, Y - vectorOther.Y, Z - vectorOther.Z, W - vectorOther.W);
        }

        public VectorFourCoord MultiplyWithVectorResult(float number)
        {
            return new VectorFourCoord(X * number, Y * number, Z * number, W * number);
        }

        public VectorFourCoord SumCoordinates(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(X + vectorOther.X, Y + vectorOther.Y, Z + vectorOther.Z, W + vectorOther.W);
        }

        public VectorFourCoord MultiplyCoordinates(VectorFourCoord vectorOther)
        {
            return new VectorFourCoord(X * vectorOther.X, Y * vectorOther.Y, Z * vectorOther.Z, W * vectorOther.W);
        }

        public VectorFourCoord TakeVectorNormalWithoutW(VectorFourCoord vectorSecond, VectorFourCoord vectorThird)
        {
            return new VectorFourCoord(vectorThird.X - X, vectorThird.Y - Y, vectorThird.Z - Z).
                MultiplyWithVectorResultWithoutW(new VectorFourCoord(vectorSecond.X - X, vectorSecond.Y - Y, vectorSecond.Z - Z)).
                TakeNormalizedVectorWithoutW();
        }
    }
}
