/*
 *
  srtm2tiff - SRTM to TIFF format converter
  
  (c) César González Segura 2015
  
  This file is part of srtm2tiff.

    srtm2tiff is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License version
	3 as published by the Free Software Foundation.

    srtm2tiff is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with srtm2tiff.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using BitMiracle.LibTiff.Classic;

namespace srtm2tiff
{
    public class TIFFWriter
    {
        public static void ExportTIFF(Bitmap bitmap, string path)
        {
            Tiff tif = Tiff.Open(path, "w");

            byte[] raster = GetImageRasterBytes(bitmap, PixelFormat.Format32bppArgb);
            tif.SetField(TiffTag.IMAGEWIDTH, bitmap.Width);
            tif.SetField(TiffTag.IMAGELENGTH, bitmap.Height);
            tif.SetField(TiffTag.COMPRESSION, Compression.NONE);
            tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);

            tif.SetField(TiffTag.ROWSPERSTRIP, bitmap.Height);

            tif.SetField(TiffTag.XRESOLUTION, bitmap.HorizontalResolution);
            tif.SetField(TiffTag.YRESOLUTION, bitmap.VerticalResolution);

            tif.SetField(TiffTag.BITSPERSAMPLE, 8);
            tif.SetField(TiffTag.SAMPLESPERPIXEL, 4);

            tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

            int stride = raster.Length / bitmap.Height;
            ConvertSamples(raster, bitmap.Width, bitmap.Height);

            for (int i = 0, offset = 0; i < bitmap.Height; i++)
            {
                tif.WriteScanline(raster, offset, i, 0);
                offset += stride;
            }

            tif.Close();
        }

        private static byte[] GetImageRasterBytes(Bitmap bitmap, PixelFormat format)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            byte[] bits = null;

            try
            {
                BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadWrite, format);
                bits = new byte[bitmapdata.Stride * bitmapdata.Height];
                System.Runtime.InteropServices.Marshal.Copy(bitmapdata.Scan0, bits, 0, bits.Length);
                bitmap.UnlockBits(bitmapdata);
            }
            catch
            {
                return null;
            }

            return bits;
        }

        private static void ConvertSamples(byte[] data, int width, int height)
        {
            int stride = data.Length / height;
            const int samplesPerPixel = 4;

            for (int y = 0; y < height; y++)
            {
                int offset = stride * y;
                int strideEnd = offset + width * samplesPerPixel;

                for (int i = offset; i < strideEnd; i += samplesPerPixel)
                {
                    byte temp = data[i + 2];
                    data[i + 2] = data[i];
                    data[i] = temp;
                }
            }
        }
    }
}
