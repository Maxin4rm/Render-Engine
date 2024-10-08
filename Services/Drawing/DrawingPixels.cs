using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DrawPolygons.Models.Primitives;
using System.Collections.Specialized;
using System.Collections;
using System.Numerics;
using System.Diagnostics;
using System.Text.Json;
using DrawPolygons.Models.RenderElements;
using DrawPolygons.Services.Lighting;
using DrawPolygons.Services.SpaceTransform;

namespace DrawPolygons.Services.Drawing
{
    public class DrawingPixels
    {
        private PictureBox pictureBox;
        private Bitmap bitmap;
        private Rectangle bitmapClientRectangle;
        private ImageLockMode imageLockMode;

        private float[,] zBuffer;

        public DrawingPixels(PictureBox pictureBox, Bitmap bitmap, Rectangle bitmapClientRectangle, ImageLockMode imageLockMode)
        {
            this.pictureBox = pictureBox;
            this.bitmap = bitmap;
            this.bitmapClientRectangle = bitmapClientRectangle;
            this.imageLockMode = imageLockMode;
            this.pictureBox.Image = bitmap;
            this.zBuffer = new float[bitmapClientRectangle.Width, bitmapClientRectangle.Height];            
        }

        public PreparingBitmapData PreparingFillAndLockBitmapBuffer()
        {
            BitmapData bitmapData = bitmap.LockBits(bitmapClientRectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            for (int i = 0; i < bitmapClientRectangle.Width; i++)
            {
                for (int j = 0; j < bitmapClientRectangle.Height; j++)
                {
                    zBuffer[i, j] = float.MaxValue;
                }
            }
            
            return new PreparingBitmapData(bitmapData, bitmapData.Scan0, Image.GetPixelFormatSize(bitmap.PixelFormat) / 8, bitmapData.Stride, zBuffer);            
        }

        public static unsafe void FillBitmapBufferDrawBackground(PreparingBitmapData preparingBitmapData, Color colorBackground)
        {
            //byte* data = (byte*)preparingBitmapData.ptr;
            //int size = preparingBitmapData.bitmapData.Height * preparingBitmapData.bitmapData.Width;
            //for (int i = 0; i < size; i++)
            //{
            //    if (preparingBitmapData.zBuffer[] == float.MaxValue)
            //    {
            //        data[0] = colorBackground.B;
            //        data[1] = colorBackground.G;
            //        data[2] = colorBackground.R;
            //        data[3] = colorBackground.A;
            //    }
            //    data += preparingBitmapData.bytesPerPixel;
            //}

            for (int i = 0; i < preparingBitmapData.bitmapData.Width; i++)
            {
                for (int j = 0; j < preparingBitmapData.bitmapData.Height; j++)
                {
                    if (preparingBitmapData.zBuffer[i, j] == float.MaxValue)
                    {
                        byte* data = (byte*)preparingBitmapData.ptr + j * preparingBitmapData.stride + i * preparingBitmapData.bytesPerPixel;

                        data[0] = colorBackground.B;
                        data[1] = colorBackground.G;
                        data[2] = colorBackground.R;
                        data[3] = colorBackground.A;
                    }
                }
            }
        }

        private static int Bound(int value, int min, int max)
        {
            if (value > max)
            {
                return max;
            }
            else if (value < min)
            {
                return min;
            }   
            
            return value;
        }       
        
        private static VectorFourCoord TakeShortenedVectorColorFromMapAndLightColor(Color colorMap, Color colorLight)
        {
            return new VectorFourCoord(colorLight.R / 255f * colorMap.R / 255f, colorLight.G / 255f * colorMap.G / 255f, colorLight.B / 255f * colorMap.B / 255f);
        }

        private static Color TakeColorResultFromPhongVectorColorShortened(VectorFourCoord colorPhongPoint)
        {
            return Color.FromArgb((int)Math.Min(colorPhongPoint.X * 255, 255), (int)Math.Min(colorPhongPoint.Y * 255, 255),
                (int)Math.Min(colorPhongPoint.Z * 255, 255));
        }

        public static unsafe void DrawFilledTriangle(object sync, PreparingBitmapData preparingBitmapData, RenderObject renderObject, VectorFourCoord[] vectorsTransformed, 
            VectorFourCoord[] vectorsWorldSpace, VerticeTexture[] verticesTexture, VectorFourCoord vectorEye, LightSource lightSource, LightSource lightBackground
        )
        {
            int minX = (int)Math.Round(Math.Min(Math.Min(vectorsTransformed[0].X, vectorsTransformed[1].X), vectorsTransformed[2].X), MidpointRounding.AwayFromZero);
            int minY = (int)Math.Round(Math.Min(Math.Min(vectorsTransformed[0].Y, vectorsTransformed[1].Y), vectorsTransformed[2].Y), MidpointRounding.AwayFromZero);
            int maxX = (int)Math.Round(Math.Max(Math.Max(vectorsTransformed[0].X, vectorsTransformed[1].X), vectorsTransformed[2].X), MidpointRounding.AwayFromZero);
            int maxY = (int)Math.Round(Math.Max(Math.Max(vectorsTransformed[0].Y, vectorsTransformed[1].Y), vectorsTransformed[2].Y), MidpointRounding.AwayFromZero);

            minX = Bound(minX, 0, preparingBitmapData.bitmapData.Width - 1);
            maxX = Bound(maxX, 0, preparingBitmapData.bitmapData.Width - 1);
            minY = Bound(minY, 0, preparingBitmapData.bitmapData.Height - 1);
            maxY = Bound(maxY, 0, preparingBitmapData.bitmapData.Height - 1);

            VerticeTexture verticeTextureInterpolated = new VerticeTexture(0, 0, 0);

            for (int y = minY; y <= maxY; ++y)
            {
                for (int x = minX; x <= maxX; ++x)
                {   
                    float w1 = ((vectorsTransformed[1].Y - vectorsTransformed[2].Y) * (x - vectorsTransformed[2].X) + (vectorsTransformed[2].X - vectorsTransformed[1].X) * (y - vectorsTransformed[2].Y)) /
                     ((vectorsTransformed[1].Y - vectorsTransformed[2].Y) * (vectorsTransformed[0].X - vectorsTransformed[2].X) + (vectorsTransformed[2].X - vectorsTransformed[1].X) * (vectorsTransformed[0].Y - vectorsTransformed[2].Y));
                    float w2 = ((vectorsTransformed[2].Y - vectorsTransformed[0].Y) * (x - vectorsTransformed[2].X) + (vectorsTransformed[0].X - vectorsTransformed[2].X) * (y - vectorsTransformed[2].Y)) /
                     ((vectorsTransformed[1].Y - vectorsTransformed[2].Y) * (vectorsTransformed[0].X - vectorsTransformed[2].X) + (vectorsTransformed[2].X - vectorsTransformed[1].X) * (vectorsTransformed[0].Y - vectorsTransformed[2].Y));
                    float w3 = 1 - w1 - w2;

                    if (w1 >= 0.0f && w2 >= 0.0f && w3 >= 0.0f)
                    {
                        float z = w1 * vectorsTransformed[0].Z + w2 * vectorsTransformed[1].Z + w3 * vectorsTransformed[2].Z;
                        if (z <= preparingBitmapData.zBuffer[x, y])
                        {
                            preparingBitmapData.zBuffer[x, y] = z;

                            VectorFourCoord vectorPointWorldSpaceInterpolated = vectorsWorldSpace[0].MultiplyWithVectorResultWithoutW(w1).
                                SumCoordinatesWithoutW(vectorsWorldSpace[1].MultiplyWithVectorResultWithoutW(w2).
                                SumCoordinatesWithoutW(vectorsWorldSpace[2].MultiplyWithVectorResultWithoutW(w3)));

                            verticeTextureInterpolated.U = verticesTexture[0].U * w1 + verticesTexture[1].U * w2 + verticesTexture[2].U * w3;
                            verticeTextureInterpolated.V = verticesTexture[0].V * w1 + verticesTexture[1].V * w2 + verticesTexture[2].V * w3;
                            verticeTextureInterpolated.W = verticesTexture[0].W * w1 + verticesTexture[1].W * w2 + verticesTexture[2].W * w3;                                                     

                            VectorFourCoord vectorLightToPoint = lightSource.VectorLight.SubstractWithVectorWithoutW(vectorPointWorldSpaceInterpolated).
                                TakeNormalizedVectorWithoutW();
                            VectorFourCoord vectorEyeToPoint = vectorEye.SubstractWithVectorWithoutW(vectorPointWorldSpaceInterpolated).
                                TakeNormalizedVectorWithoutW();

                            lock (sync)
                            {
                                verticeTextureInterpolated.U = verticeTextureInterpolated.U * renderObject.BitmapDiffuse.Width;
                                verticeTextureInterpolated.V = verticeTextureInterpolated.V * renderObject.BitmapDiffuse.Height;
                            }

                            int u = (int) ((float) verticeTextureInterpolated.U / verticeTextureInterpolated.W); 
                            int v = (int) ((float) verticeTextureInterpolated.V / verticeTextureInterpolated.W);

                            Color colorMapDiffuse;
                            Color colorMapSpecular;
                            Color colorPixeNormal;
                            lock (sync)
                            {
                                u = Math.Max(0, Math.Min(u, renderObject.BitmapDiffuse.Height - 1));
                                v = Math.Max(0, Math.Min(renderObject.BitmapDiffuse.Width - v, renderObject.BitmapDiffuse.Width - 1));

                                colorMapDiffuse = renderObject.BitmapDiffuse.GetPixel(u, v);
                                colorMapSpecular = renderObject.BitmapSpecular.GetPixel(u, v);
                                colorPixeNormal = renderObject.BitmapNormals.GetPixel(u, v);
                            }

                            VectorFourCoord vectorPixelNormal = renderObject.MatrixRotateZ.Myltiply(renderObject.MatrixRotateY.Myltiply(renderObject.MatrixRotateX.
                                Myltiply(new VectorFourCoord((colorPixeNormal.R / 255f * 2f) - 1f, (colorPixeNormal.G / 255f * 2f) - 1f, 
                                (colorPixeNormal.B / 255f * 2f) - 1f)
                            )));



                            VectorFourCoord colorPhongPoint = PhongLighting.TakePhongLightingColorForPointShortened(vectorLightToPoint, vectorPixelNormal,
                                renderObject.CoefficientLightBackground, TakeShortenedVectorColorFromMapAndLightColor(colorMapDiffuse, lightBackground.Color), renderObject.CoefficientLightDiffuse,
                                TakeShortenedVectorColorFromMapAndLightColor(colorMapDiffuse, lightSource.Color), vectorEyeToPoint, renderObject.CoefficientLightDistributeSpecular, 
                                renderObject.CoefficientLightSpecular, TakeShortenedVectorColorFromMapAndLightColor(colorMapSpecular, lightSource.Color));

                            Color colorResultPoint = TakeColorResultFromPhongVectorColorShortened(colorPhongPoint);

                            byte* data = (byte*)preparingBitmapData.ptr + y * preparingBitmapData.stride + x * preparingBitmapData.bytesPerPixel;

                            data[0] = colorResultPoint.B;
                            data[1] = colorResultPoint.G;
                            data[2] = colorResultPoint.R;
                            data[3] = colorResultPoint.A;
                        }
                    }
                }
            }
        }

        private static unsafe void FillBitmapBufferDrawPoints(PreparingBitmapData preparingBitmapData, Color colorLines, List<VectorFourCoord> vectorsTransformed)
        {
            for (int i = 0; i < vectorsTransformed.Count; i++)
            {
                byte* data = (byte*)preparingBitmapData.ptr + (int)vectorsTransformed[i].Y * preparingBitmapData.stride + (int)vectorsTransformed[i].X * preparingBitmapData.bytesPerPixel;
                data[0] = colorLines.B;
                data[1] = colorLines.G;
                data[2] = colorLines.R;
                data[3] = colorLines.A;
            }
        }

        private static unsafe void DrawLineBresenham(PreparingBitmapData preparingBitmapData, Color colorLines, int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                int pos = y0 * preparingBitmapData.bitmapData.Width + x0;
                byte* data = (byte*)preparingBitmapData.ptr + y0 * preparingBitmapData.stride + x0 * preparingBitmapData.bytesPerPixel;
                if (x0 >= 0 && x0 < preparingBitmapData.bitmapData.Width && y0 >= 0 && y0 < preparingBitmapData.bitmapData.Height)
                {
                    data[0] = colorLines.B;
                    data[1] = colorLines.G;
                    data[2] = colorLines.R;
                    data[3] = colorLines.A;
                    //preparingBitmapData.redrawPixels.Set(pos, true);
                }

                if (x0 == x1 && y0 == y1)
                    break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }



        public static void FillBitmapBufferDrawTriangle(PreparingBitmapData preparingBitmapData, Color colorLines, VectorFourCoord[] vectorsTransformed)
        {
            DrawLineBresenham(preparingBitmapData, colorLines, (int)vectorsTransformed[0].X, (int)vectorsTransformed[0].Y, (int)vectorsTransformed[1].X, (int)vectorsTransformed[1].Y);
            DrawLineBresenham(preparingBitmapData, colorLines, (int)vectorsTransformed[1].X, (int)vectorsTransformed[1].Y, (int)vectorsTransformed[2].X, (int)vectorsTransformed[2].Y);
            DrawLineBresenham(preparingBitmapData, colorLines, (int)vectorsTransformed[2].X, (int)vectorsTransformed[2].Y, (int)vectorsTransformed[0].X, (int)vectorsTransformed[0].Y);
        }

        public void ShowAndUnlockBitmapBuffer(PreparingBitmapData preparingBitmapData)
        {
            bitmap.UnlockBits(preparingBitmapData.bitmapData);
            pictureBox.Refresh();
        }
    }
}
