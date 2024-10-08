using DrawPolygons.Models.Primitives;
using DrawPolygons.Models.RenderElements;
using DrawPolygons.Services.Drawing;
using DrawPolygons.Services.ParserObjectFile;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices;
using DrawPolygons.Services.SpaceTransform;
using DrawPolygons.Services;
using DrawPolygons.Services.Lighting;

namespace Lab1_DrawPolygons_SiSharp_V1
{
    public partial class FormMain : Form
    {
        private const string PATH_TO_OBJECTS_DIRECTORY = "..\\..\\..\\FilesObject";
        private const string FILE_NAME_MODEL_NAME = "model.obj";
        private const string FILE_NAME_MAP_SPECULAR = "mapSpecular.png";
        private const string FILE_NAME_MAP_DIFFUSE = "mapDiffuse.png";
        private const string FILE_NAME_MAP_NORMALS = "mapNormals.png";

        readonly bool isCloseProgram;

        private readonly ParserModelFile parserObjFile;
        private readonly RenderObject? renderObject;

        private readonly Color colorSceneBackground;

        private readonly ObserverSpaceTransform observerSpaceTransform;
    
        private readonly ProjectionSpaceTransform projectionSpaceTransform;
        private readonly ViewportSpaceTransform viewportSpaceTransform;

        private readonly MatrixFourByFour matrixObserver;
        private readonly MatrixFourByFour matrixProjection;
        private readonly MatrixFourByFour matrixViewport;

        private readonly DrawingPixels drawingPixels;
        private readonly LightSource lightSource;
        private readonly LightSource lightBackground;



        private static Bitmap TryCreateBitmapFromFile(string pathBitmapFile)
        {
            Bitmap bitmap;

            try
            {
                bitmap = new Bitmap(pathBitmapFile);
            }          
            catch (Exception)
            {
                throw new Exception($"File \"{pathBitmapFile}\" is not found!");
            }

            return bitmap;
        }

        private static float ConvertDegreesToRadians(float angle)
        {
            return (float)((Math.PI / 180) * angle);
        }

        public FormMain()
        {
            isCloseProgram = false;
            InitializeComponent();

            parserObjFile = ParserModelFile.CreateInstance();

            renderObject = new RenderObject(PATH_TO_OBJECTS_DIRECTORY + "\\" + "Head");            

            VectorFourCoord vectorTranslation = new(0, 0, 0);
            VectorFourCoord vectorScale = new(1, 1, 1);
            float angleDegreesX = 0;
            float angleDegreesY = -20;
            float angleDegreesZ = 0;

            VectorFourCoord vectorEye = new(0, 0, 2f);
            VectorFourCoord vectorTarget = new(0, 0, 0);
            VectorFourCoord vectorUp = new(0, 1, 0);

            float cameraHeight = pictureBox.Size.Height;
            float cameraWidth = pictureBox.Size.Width;
            float fovDegrees = 90;
            float zNear = 0.1f;
            float zFar = 100.0f;

            float windowHeight = pictureBox.Size.Height;
            float windowWidth = pictureBox.Size.Width;
            float xMin = 0;
            float yMin = 0;

            renderObject.CoefficientLightBackground = 0.4f;
            renderObject.CoefficientLightDiffuse = 0.5f;
            renderObject.CoefficientLightSpecular = 1.0f;
            renderObject.CoefficientLightDistributeSpecular = 10;
            renderObject.Color = Color.FromArgb(0, 0, 0);

            renderObject.WorldSpaceTransorm = new WorldSpaceTransform(vectorTranslation, vectorScale, ConvertDegreesToRadians(angleDegreesX),
                ConvertDegreesToRadians(angleDegreesY), ConvertDegreesToRadians(angleDegreesZ));
            renderObject.MatrixTranslation = renderObject.WorldSpaceTransorm.TakeMatrixTranslation();
            renderObject.MatrixRotateZ = renderObject.WorldSpaceTransorm.TakeMatrixRotateZ();
            renderObject.MatrixRotateY = renderObject.WorldSpaceTransorm.TakeMatrixRotateY();
            renderObject.MatrixRotateX = renderObject.WorldSpaceTransorm.TakeMatrixRotateX();
            renderObject.MatrixScale = renderObject.WorldSpaceTransorm.TakeMatrixScale();

            observerSpaceTransform = new ObserverSpaceTransform(vectorEye, vectorTarget, vectorUp);
            matrixObserver = observerSpaceTransform.TakeViewMatrix();

            projectionSpaceTransform = new ProjectionSpaceTransform(cameraHeight, cameraWidth, ConvertDegreesToRadians(fovDegrees), zNear, zFar);
            matrixProjection = projectionSpaceTransform.TakeMatrixProjection();

            viewportSpaceTransform = new ViewportSpaceTransform(windowWidth, windowHeight, xMin, yMin);
            matrixViewport = viewportSpaceTransform.TakeMatrixViewport();

            lightSource = new LightSource(new VectorFourCoord(-50, 10, 5), Color.FromArgb(255, 255, 255));
            lightBackground = new LightSource(new VectorFourCoord(0, 0, 0), Color.FromArgb(255, 255, 255));

            colorSceneBackground = Color.FromArgb(0, 0, 0);

            Bitmap bitmap = new(pictureBox.Size.Width, pictureBox.Size.Height, PixelFormat.Format32bppArgb);
            Rectangle bitmapClientRectangle = new(0, 0, bitmap.Width, bitmap.Height);
            pictureBox.Image = bitmap;
            drawingPixels = new DrawingPixels(pictureBox, bitmap, bitmapClientRectangle, ImageLockMode.ReadWrite);

            try
            {
                renderObject = parserObjFile.ParseObjFile(renderObject, renderObject.PathToObjectDirectory + "\\" + FILE_NAME_MODEL_NAME, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "ParserModelFile error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly
                );

                isCloseProgram = true;
                return;
            }

            try
            {
                renderObject.BitmapDiffuse = TryCreateBitmapFromFile(renderObject.PathToObjectDirectory + "\\" + FILE_NAME_MAP_DIFFUSE);
                renderObject.BitmapSpecular = TryCreateBitmapFromFile(renderObject.PathToObjectDirectory + "\\" + FILE_NAME_MAP_SPECULAR);
                renderObject.BitmapNormals = TryCreateBitmapFromFile(renderObject.PathToObjectDirectory + "\\" + FILE_NAME_MAP_NORMALS);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Maps object name error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly
                );

                isCloseProgram = true;
                return;
            }

            DrawRenderObject(renderObject);
        }

        private void DrawRenderObject(RenderObject renderObject)
        {
            MatrixFourByFour matrixModel = renderObject.MatrixTranslation.Myltiply(renderObject.MatrixRotateZ.Myltiply(
                renderObject.MatrixRotateY.Myltiply(renderObject.MatrixRotateX.Myltiply(renderObject.MatrixScale))));
            MatrixFourByFour matrixObserverProjection = matrixProjection.Myltiply(matrixObserver);

            PreparingBitmapData preparingBitmapData = drawingPixels.PreparingFillAndLockBitmapBuffer();

            object sync = new();
            Parallel.ForEach(renderObject.TrianglesIndexes, triangleIndex =>
            //for (int i = 0; i < renderObject.TrianglesIndexes.Length; i++)
            {
                VectorFourCoord[] vectorsTransformed = new VectorFourCoord[3];
                //VectorFourCoord[] vectorsNormalWeightedWorldSpace = new VectorFourCoord[3];
                for (int j = 0; j < 3; j++)
                {
                    //int indexCurrent = renderObject.TrianglesIndexes[i].VerticesGeometricIndexes[j] - 1;
                    int indexCurrent = triangleIndex.VerticesGeometricIndexes[j] - 1;

                    vectorsTransformed[j] = matrixModel.Myltiply(
                        new VectorFourCoord(
                            renderObject.VerticesGeometric[indexCurrent].X,
                            renderObject.VerticesGeometric[indexCurrent].Y,
                            renderObject.VerticesGeometric[indexCurrent].Z,
                            renderObject.VerticesGeometric[indexCurrent].W
                    ));

                    //vectorsNormalWeightedWorldSpace[j] = matrixModel.Myltiply(renderObject.VectorsNormalWeighted[renderObject.TrianglesIndexes[i].VectorsNormalWeightedIndexes[j] - 1]);
                    //vectorsNormalWeightedWorldSpace[j] = matrixModel.Myltiply(renderObject.VectorsNormalWeighted[triangleIndex.VectorsNormalWeightedIndexes[j] - 1]);
                }

                VectorFourCoord[] vectorsWorldSpace = new VectorFourCoord[3];
                for (int j = 0; j < vectorsTransformed.Length; j++)
                {
                    vectorsWorldSpace[j] = vectorsTransformed[j];
                }

                VectorFourCoord vectorTriangleNormal = vectorsTransformed[0].TakeVectorNormalWithoutW(vectorsTransformed[1], vectorsTransformed[2]);

                if (RejectingInvisibleTriangles.IsVisibleTriangle(vectorTriangleNormal, observerSpaceTransform.Eye, vectorsTransformed[0]))
                {
                    VerticeTexture[] verticesTexture = new VerticeTexture[3];

                    for (int j = 0; j < 3; j++)
                    {
                        vectorsTransformed[j] = matrixObserverProjection.Myltiply(vectorsTransformed[j]);

                        if (vectorsTransformed[j].W > 0.0f) 
                        {
                            verticesTexture[j] = new VerticeTexture(
                                renderObject.VerticesTexture[triangleIndex.VerticesTextureIndexes[j] - 1].U / vectorsTransformed[j].W,
                                renderObject.VerticesTexture[triangleIndex.VerticesTextureIndexes[j] - 1].V / vectorsTransformed[j].W,
                                renderObject.VerticesTexture[triangleIndex.VerticesTextureIndexes[j] - 1].W / vectorsTransformed[j].W
                                //renderObject.VerticesTexture[renderObject.TrianglesIndexes[i].VerticesTextureIndexes[j] - 1].U / vectorsTransformed[j].W,
                                //renderObject.VerticesTexture[renderObject.TrianglesIndexes[i].VerticesTextureIndexes[j] - 1].V / vectorsTransformed[j].W,
                                //renderObject.VerticesTexture[renderObject.TrianglesIndexes[i].VerticesTextureIndexes[j] - 1].W / vectorsTransformed[j].W
                            );
                            
                            vectorsTransformed[j].X = vectorsTransformed[j].X / vectorsTransformed[j].W;
                            vectorsTransformed[j].Y = vectorsTransformed[j].Y / vectorsTransformed[j].W;
                            vectorsTransformed[j].Z = vectorsTransformed[j].Z / vectorsTransformed[j].W;
                            vectorsTransformed[j].W = 1;
                            vectorsTransformed[j] = matrixViewport.Myltiply(vectorsTransformed[j]);
                        }
                        else
                        {
                            goto l1;
                        }
                    }

                    DrawingPixels.DrawFilledTriangle(sync, preparingBitmapData, renderObject, vectorsTransformed, vectorsWorldSpace, verticesTexture,
                        observerSpaceTransform.Eye, lightSource, lightBackground);
                }
            l1:;
            }
            );

            DrawingPixels.FillBitmapBufferDrawBackground(preparingBitmapData, colorSceneBackground);

            drawingPixels.ShowAndUnlockBitmapBuffer(preparingBitmapData);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            //while (renderObject != null)
            //{
            //    worldSpaceTransorm.AngleRadiansY = worldSpaceTransorm.AngleRadiansY - 0.05f;
            //    matrixRotateY = worldSpaceTransorm.TakeMatrixRotateY();
            //    DrawRenderObject(renderObject);
            //}
            if (renderObject != null)
            {
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                {
                    renderObject.WorldSpaceTransorm.AngleRadiansY = renderObject.WorldSpaceTransorm.AngleRadiansY - 0.1f;
                    renderObject.MatrixRotateY = renderObject.WorldSpaceTransorm.TakeMatrixRotateY();
                }
                else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                {
                    renderObject.WorldSpaceTransorm.AngleRadiansY = renderObject.WorldSpaceTransorm.AngleRadiansY + 0.1f;
                    renderObject.MatrixRotateY = renderObject.WorldSpaceTransorm.TakeMatrixRotateY();
                }
                else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
                {
                    renderObject.WorldSpaceTransorm.AngleRadiansX = renderObject.WorldSpaceTransorm.AngleRadiansX - 0.1f;
                    renderObject.MatrixRotateX = renderObject.WorldSpaceTransorm.TakeMatrixRotateX();
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
                {
                    renderObject.WorldSpaceTransorm.AngleRadiansX = renderObject.WorldSpaceTransorm.AngleRadiansX + 0.1f;
                    renderObject.MatrixRotateX = renderObject.WorldSpaceTransorm.TakeMatrixRotateX();
                }
                else if (e.KeyCode == Keys.Q)
                {
                    renderObject.WorldSpaceTransorm.AngleRadiansZ = renderObject.WorldSpaceTransorm.AngleRadiansZ + 0.1f;
                    renderObject.MatrixRotateZ = renderObject.WorldSpaceTransorm.TakeMatrixRotateZ();
                }
                else if (e.KeyCode == Keys.E)
                {
                    renderObject.WorldSpaceTransorm.AngleRadiansZ = renderObject.WorldSpaceTransorm.AngleRadiansZ - 0.1f;
                    renderObject.MatrixRotateZ = renderObject.WorldSpaceTransorm.TakeMatrixRotateZ();
                }                

                DrawRenderObject(renderObject);
            }

            //worldSpaceTransorm.AngleRadiansY = worldSpaceTransorm.AngleRadiansY - 0.1f;
            //matrixRotateY = worldSpaceTransorm.TakeMatrixRotateY();
            //DrawRenderObject(renderObject);
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (isCloseProgram)
            {
                Close();
            }
        }
    }
}
