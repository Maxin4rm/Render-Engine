using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Models.Primitives
{
    public class MatrixFourByFour
    {
        private const int MATRIX_ROWS = 4;
        private const int MATRIX_COLUMNS = 4;

        public float[,] Values { get; set; }

        public MatrixFourByFour(float[,] values)
        {
            if (values.GetLength(0) == MATRIX_ROWS && values.GetLength(1) == MATRIX_COLUMNS)
            {
                Values = values;
            }
            else
            {
                throw new Exception($"Matrix should have {MATRIX_ROWS} rows and {MATRIX_COLUMNS} columns!");
            }
        }

        public MatrixFourByFour Myltiply(MatrixFourByFour matrixSecond)
        {
            int valuesRows = this.Values.GetLength(0);
            int valuesColumns = this.Values.GetLength(1);
            int matrixSecondRows = matrixSecond.Values.GetLength(0);
            int matrixSecondColumns = matrixSecond.Values.GetLength(1);

            if (valuesColumns != matrixSecondRows)
            {
                throw new ArgumentException("The number of columns in matrix first should be equal to the number of rows in matrix second!");
            }

            float[,] matrixResult = new float[valuesRows, matrixSecondColumns];

            for (int i = 0; i < valuesRows; i++)
            {
                for (int j = 0; j < matrixSecondColumns; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < valuesColumns; k++)
                    {
                        sum = sum + this.Values[i, k] * matrixSecond.Values[k, j];
                    }
                    matrixResult[i, j] = sum;
                }
            }

            return new MatrixFourByFour(matrixResult);
        }

        public VectorFourCoord Myltiply(VectorFourCoord vectorOther)
        {

            return new VectorFourCoord(
                this.Values[0, 0] * vectorOther.X + this.Values[0, 1] * vectorOther.Y + this.Values[0, 2] * vectorOther.Z + this.Values[0, 3] * vectorOther.W,
                this.Values[1, 0] * vectorOther.X + this.Values[1, 1] * vectorOther.Y + this.Values[1, 2] * vectorOther.Z + this.Values[1, 3] * vectorOther.W,
                this.Values[2, 0] * vectorOther.X + this.Values[2, 1] * vectorOther.Y + this.Values[2, 2] * vectorOther.Z + this.Values[2, 3] * vectorOther.W,
                this.Values[3, 0] * vectorOther.X + this.Values[3, 1] * vectorOther.Y + this.Values[3, 2] * vectorOther.Z + this.Values[3, 3] * vectorOther.W
            );
        }

        public MatrixFourByFour Transpose()
        {
            float[,] matrixNewValues = new float[MATRIX_COLUMNS, MATRIX_ROWS];

            for (int i = 0; i < MATRIX_COLUMNS; i++)
            {
                for (int j = 0; j < MATRIX_ROWS; j++)
                {
                    matrixNewValues[i, j] = this.Values[j, i];
                }
            }

            return new MatrixFourByFour(matrixNewValues);
        }
    }
}
