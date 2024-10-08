using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawPolygons.Models.Primitives;
using DrawPolygons.Services.SpaceTransform;

namespace DrawPolygons.Models.RenderElements
{
    public class RenderObject
    {
        public Bitmap BitmapDiffuse { get; set; }
        public Bitmap BitmapSpecular { get; set; }
        public Bitmap BitmapNormals { get; set; }

        public VerticeGeometric[] VerticesGeometric { get; set; }
        public VerticeTexture[] VerticesTexture { get; set; }
        public VectorFourCoord[] VectorsNormal { get; set; }
        public VectorFourCoord[] VectorsNormalWeighted { get; set; }
        public TriangleIndexes[] TrianglesIndexes { get; set; }

        public WorldSpaceTransform WorldSpaceTransorm { get; set; }
        public MatrixFourByFour MatrixTranslation { get; set; }  
        public MatrixFourByFour MatrixRotateZ { get; set; }  
        public MatrixFourByFour MatrixRotateY { get; set; }  
        public MatrixFourByFour MatrixRotateX { get; set; }  
        public MatrixFourByFour MatrixScale { get; set; }  

        public float CoefficientLightBackground { get; set; }
        public float CoefficientLightDiffuse { get; set; }
        public float CoefficientLightSpecular { get; set; }
        public float CoefficientLightDistributeSpecular { get; set; }

        public Color Color { get; set; }

        public string PathToObjectDirectory { get; set; }


        public RenderObject(string pathToObjectDirectory = "")
        {
            BitmapDiffuse = new Bitmap(1, 1);
            BitmapSpecular = new Bitmap(1, 1);
            BitmapNormals = new Bitmap(1, 1);

            VerticesGeometric = new VerticeGeometric[0];
            VerticesTexture = new VerticeTexture[0];
            VectorsNormal = new VectorFourCoord[0];
            VectorsNormalWeighted = new VectorFourCoord[0];
            TrianglesIndexes = new TriangleIndexes[0];

            WorldSpaceTransorm = new WorldSpaceTransform(new VectorFourCoord(0,0,0), new VectorFourCoord(1,1,1), 0, 0, 0);
            MatrixTranslation = WorldSpaceTransorm.TakeMatrixTranslation();
            MatrixRotateZ = WorldSpaceTransorm.TakeMatrixRotateZ();
            MatrixRotateY = WorldSpaceTransorm.TakeMatrixRotateY();
            MatrixRotateX = WorldSpaceTransorm.TakeMatrixRotateX();
            MatrixScale = WorldSpaceTransorm.TakeMatrixScale();

            CoefficientLightBackground = 1;
            CoefficientLightDiffuse = 1;
            CoefficientLightSpecular = 1;
            CoefficientLightDistributeSpecular = 1;

            PathToObjectDirectory = pathToObjectDirectory;

            Color = Color.FromArgb(255, 255, 255);            
        }
    }
}
