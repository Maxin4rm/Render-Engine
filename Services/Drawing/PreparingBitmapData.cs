using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DrawPolygons.Services.Drawing
{
    public class PreparingBitmapData
    {
        public BitmapData bitmapData { get; set; }
        public IntPtr ptr { get; set; }
        public int bytesPerPixel { get; set; }
        public int stride { get; set; }
        public float[,] zBuffer { get; set; }

        public PreparingBitmapData(BitmapData bitmapData, IntPtr ptr, int bytesPerPixel, int stride, float[,] zBuffer)
        {
            this.bitmapData = bitmapData;
            this.ptr = ptr;
            this.bytesPerPixel = bytesPerPixel;
            this.stride = stride;
            this.zBuffer = zBuffer;
        }
    }
}
