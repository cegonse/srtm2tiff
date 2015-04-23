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
using System.IO;

namespace srtm2tiff
{
    public enum SRTMFormat
    {
        SRTM3,
        SRTM1,
        Unknown
    }

    public class SRTMReader
    {
        public SRTMReader(BinaryReader reader, SRTMFormat format)
        {
            if (reader == null)
            {
                throw new NullReferenceException("El BinaryReader no debe ser null.");
            }

            _binaryReader = reader;
            _format = format;

            if (_format == SRTMFormat.SRTM1)
            {
                _tileSize = 3601;
            }
            else if (_format == SRTMFormat.SRTM3)
            {
                _tileSize = 1201;
            }
            else if (_format == SRTMFormat.Unknown)
            {
                throw new FormatException("No se ha podido reconocer el formato del archivo HGT.");
            }

            _heightData = new Int16[_tileSize, _tileSize];
        }

        public void ReadAllData(bool buffer = true)
        {
            bool littleEndian = BitConverter.IsLittleEndian;
            long bytes = 0;
            Int16[] data = null;
            int bufferIndex = 0;

            if (buffer)
            {
                data = new Int16[_tileSize * _tileSize];
                byte[] bBuffer = _binaryReader.ReadBytes(_tileSize * _tileSize * 2);

                for (int i = 0; i < bBuffer.Length; i += 2)
                {
                    data[bufferIndex] = BitConverter.ToInt16(bBuffer, i);
                    bufferIndex++;
                }
            }

            bufferIndex = 0;

            for (int i = 0; i < _tileSize; i++)
            {
                for (int j = 0; j < _tileSize; j++)
                {
                    Int16 temp = 0;

                    if (buffer)
                    {
                        temp = data[bufferIndex];
                        bufferIndex++;
                    }
                    else
                    {
                        temp = _binaryReader.ReadInt16();
                    }

                    if (littleEndian)
                    {
                        byte[] tempByteArray = BitConverter.GetBytes(temp);

                        byte tempLow = tempByteArray[0];
                        byte tempHigh = tempByteArray[1];
                        tempByteArray[0] = tempHigh;
                        tempByteArray[1] = tempLow;

                        temp = BitConverter.ToInt16(tempByteArray, 0);
                    }

                    if (temp == -32768)
                    {
                        temp = 0;
                    }

                    _heightData[i, j] = temp;
                    bytes += 2;
                }
            }
        }

        public Int16[,] GetElevationDataMatrix()
        {
            return _heightData;
        }

        public Int16 GetElevationAt(int x, int y)
        {
            return _heightData[x, y];
        }

        public int GetTileSize()
        {
            return _tileSize;
        }

        private BinaryReader _binaryReader;
        private Int16[,] _heightData;
        private SRTMFormat _format;
        private int _tileSize;
    }
}
