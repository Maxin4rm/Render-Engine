using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPolygons.Models.Primitives
{
    public class TriangleIndexes
    {
        private const int COUNT_VERTICES = 3;

        public int[] VerticesGeometricIndexes { get; set; }
        public int[] VerticesTextureIndexes { get; set; }
        public int[] VectorsNormalIndexes { get; set; }
        public int[] VectorsNormalWeightedIndexes { get; set; }

        public TriangleIndexes(int[] verticesGeometricIndexes, int[] verticesTextureIndexes, int[] vectorsNormalIndexes, int[] vectorsNormalWeightedIndexes)
        {
            if (verticesGeometricIndexes.Length == COUNT_VERTICES && verticesTextureIndexes.Length == COUNT_VERTICES && vectorsNormalIndexes.Length == COUNT_VERTICES && vectorsNormalWeightedIndexes.Length == COUNT_VERTICES)
            {
                VerticesGeometricIndexes = verticesGeometricIndexes;
                VerticesTextureIndexes = verticesTextureIndexes;
                VectorsNormalIndexes = vectorsNormalIndexes;
                VectorsNormalWeightedIndexes = vectorsNormalWeightedIndexes;
            }
            else
            {
                throw new Exception("Triangle should have 3 vertices!");
            }
        }
    }
}
