/*
 *
  srtm2tiff - SRTM to TIFF format converter
  
  (c) César González Segura 2015
  
  This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License version
	3 as published by the Free Software Foundation.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace srtm2tiff
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                WriteInstructions();
            }
            else
            {
                bool verbose = false;
                int i = 0, count = args.Length;

                if (args[0] == "-verbose")
                {
                    verbose = true;
                    i++;

                    if (args.Length == 1)
                    {
                        WriteInstructions();
                        Environment.Exit(0);
                    }
                }

                for (; i < count; i++)
                {
                    try
                    {
                        FileStream file = File.Open(args[i], FileMode.Open);
                        BinaryReader reader = new BinaryReader(file);

                        SRTMFormat format = SRTMFormat.Unknown;

                        if (file.Length == 3601 * 3601 * sizeof(Int16))
                        {
                            format = SRTMFormat.SRTM1;
                        }
                        else if (file.Length == 1201 * 1201 * sizeof(Int16))
                        {
                            format = SRTMFormat.SRTM3;
                        }

                        if (verbose)
                        {
                            Console.WriteLine("Reading SRTM file: " + args[i]);
                            Console.WriteLine(" - Length: " + file.Length + " Bytes");
                            Console.WriteLine(" - Format: " + format.ToString() + Environment.NewLine);
                        }

                        SRTMReader srtm = new SRTMReader(reader, format);
                        srtm.ReadAllData(false);

                        BitmapTerrain bmpTerrain = new BitmapTerrain(srtm.GetTileSize(), srtm.GetTileSize());
                        bmpTerrain.ConvertFromSRTM(srtm, BitmapChannels.ChannelRed | BitmapChannels.ChannelGreen);

                        if (verbose)
                        {
                            Console.WriteLine("Writing TIFF file from SRTM file...");
                        }

                        string tiffPath = args[i];
                        int dot = tiffPath.IndexOf('.');
                        tiffPath = tiffPath.Substring(0, dot);
                        tiffPath += ".tiff";

                        TIFFWriter.ExportTIFF(bmpTerrain.GetBitmap(), tiffPath);

                        if (verbose)
                        {
                            Console.WriteLine("Success" + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (verbose)
                        {
                            Console.WriteLine("Couldn't convert SRTM file:");
                            Console.WriteLine(ex.Message + Environment.NewLine);
                        }
                    }
                }
            }
        }

        static void WriteInstructions()
        {
            Console.WriteLine("srtm2tiff, created by Cesar Gonzalez Segura.");
            Console.WriteLine("  srtm2tiff -verbose [file1] [file2] ...");
        }
    }
}
