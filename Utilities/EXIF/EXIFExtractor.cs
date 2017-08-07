#region License : arachnode.net

// // Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
// //  
// // Permission is hereby granted, upon purchase, to any person
// // obtaining a copy of this software and associated documentation
// // files (the "Software"), to deal in the Software without
// // restriction, including without limitation the rights to use,
// // copy, merge and modify copies of the Software, and to permit persons
// // to whom the Software is furnished to do so, subject to the following
// // conditions:
// // 
// // LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// // 
// // The above copyright notice and this permission notice shall be
// // included in all copies or substantial portions of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// // OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// // NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// // HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// // FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;

#endregion

namespace Arachnode.Utilities.EXIF
{
    /// <summary>
    /// 	EXIFextractor Class
    /// </summary>
    public class EXIFExtractor : IEnumerable
    {
        //
        private readonly Image _image;
        //
        //
        private readonly EXIFTranslator _myHash;
        //
        private readonly Hashtable _properties;
        //

        //
        private readonly string sp;
        private string _data;
        private string msp = "";

        /// <summary>
        /// </summary>
        /// <param name = "bmp"></param>
        /// <param name = "sp"></param>
        public EXIFExtractor(ref Image bmp, string sp)
        {
            _properties = new Hashtable();
            //
            _image = bmp;
            this.sp = sp;
            //
            _myHash = new EXIFTranslator();
            buildDB(_image.PropertyItems);
        }

        public EXIFExtractor(Image image, string sp, string msp)
        {
            _properties = new Hashtable();
            this.sp = sp;
            this.msp = msp;
            _image = image;
            //				
            _myHash = new EXIFTranslator();
            buildDB(image.PropertyItems);
        }

        public EXIFExtractor(string file, string sp, string msp)
        {
            _properties = new Hashtable();
            this.sp = sp;
            this.msp = msp;

            _myHash = new EXIFTranslator();
            //				
            buildDB(GetExifProperties(file));
        }

        /// <summary>
        /// 	Get the individual property value by supplying property name
        /// 	These are the valid property names :
        /// 
        /// 	"Exif IFD"
        /// 	"Gps IFD"
        /// 	"New Subfile Type"
        /// 	"Subfile Type"
        /// 	"Image Width"
        /// 	"Image Height"
        /// 	"Bits Per Sample"
        /// 	"Compression"
        /// 	"Photometric Interp"
        /// 	"Thresh Holding"
        /// 	"Cell Width"
        /// 	"Cell Height"
        /// 	"Fill Order"
        /// 	"Document Name"
        /// 	"Image Description"
        /// 	"Equip Make"
        /// 	"Equip Model"
        /// 	"Strip Offsets"
        /// 	"Orientation"
        /// 	"Samples PerPixel"
        /// 	"Rows Per Strip"
        /// 	"Strip Bytes Count"
        /// 	"Min Sample Value"
        /// 	"Max Sample Value"
        /// 	"X Resolution"
        /// 	"Y Resolution"
        /// 	"Planar Config"
        /// 	"Page Name"
        /// 	"X Position"
        /// 	"Y Position"
        /// 	"Free Offset"
        /// 	"Free Byte Counts"
        /// 	"Gray Response Unit"
        /// 	"Gray Response Curve"
        /// 	"T4 Option"
        /// 	"T6 Option"
        /// 	"Resolution Unit"
        /// 	"Page Number"
        /// 	"Transfer Funcition"
        /// 	"Software Used"
        /// 	"Date Time"
        /// 	"Artist"
        /// 	"Host Computer"
        /// 	"Predictor"
        /// 	"White Point"
        /// 	"Primary Chromaticities"
        /// 	"ColorMap"
        /// 	"Halftone Hints"
        /// 	"Tile Width"
        /// 	"Tile Length"
        /// 	"Tile Offset"
        /// 	"Tile ByteCounts"
        /// 	"InkSet"
        /// 	"Ink Names"
        /// 	"Number Of Inks"
        /// 	"Dot Range"
        /// 	"Target Printer"
        /// 	"Extra Samples"
        /// 	"Sample Format"
        /// 	"S Min Sample Value"
        /// 	"S Max Sample Value"
        /// 	"Transfer Range"
        /// 	"JPEG Proc"
        /// 	"JPEG InterFormat"
        /// 	"JPEG InterLength"
        /// 	"JPEG RestartInterval"
        /// 	"JPEG LosslessPredictors"
        /// 	"JPEG PointTransforms"
        /// 	"JPEG QTables"
        /// 	"JPEG DCTables"
        /// 	"JPEG ACTables"
        /// 	"YCbCr Coefficients"
        /// 	"YCbCr Subsampling"
        /// 	"YCbCr Positioning"
        /// 	"REF Black White"
        /// 	"ICC Profile"
        /// 	"Gamma"
        /// 	"ICC Profile Descriptor"
        /// 	"SRGB RenderingIntent"
        /// 	"Image Title"
        /// 	"Copyright"
        /// 	"Resolution X Unit"
        /// 	"Resolution Y Unit"
        /// 	"Resolution X LengthUnit"
        /// 	"Resolution Y LengthUnit"
        /// 	"Print Flags"
        /// 	"Print Flags Version"
        /// 	"Print Flags Crop"
        /// 	"Print Flags Bleed Width"
        /// 	"Print Flags Bleed Width Scale"
        /// 	"Halftone LPI"
        /// 	"Halftone LPIUnit"
        /// 	"Halftone Degree"
        /// 	"Halftone Shape"
        /// 	"Halftone Misc"
        /// 	"Halftone Screen"
        /// 	"JPEG Quality"
        /// 	"Grid Size"
        /// 	"Thumbnail Format"
        /// 	"Thumbnail Width"
        /// 	"Thumbnail Height"
        /// 	"Thumbnail ColorDepth"
        /// 	"Thumbnail Planes"
        /// 	"Thumbnail RawBytes"
        /// 	"Thumbnail Size"
        /// 	"Thumbnail CompressedSize"
        /// 	"Color Transfer Function"
        /// 	"Thumbnail Data"
        /// 	"Thumbnail ImageWidth"
        /// 	"Thumbnail ImageHeight"
        /// 	"Thumbnail BitsPerSample"
        /// 	"Thumbnail Compression"
        /// 	"Thumbnail PhotometricInterp"
        /// 	"Thumbnail ImageDescription"
        /// 	"Thumbnail EquipMake"
        /// 	"Thumbnail EquipModel"
        /// 	"Thumbnail StripOffsets"
        /// 	"Thumbnail Orientation"
        /// 	"Thumbnail SamplesPerPixel"
        /// 	"Thumbnail RowsPerStrip"
        /// 	"Thumbnail StripBytesCount"
        /// 	"Thumbnail ResolutionX"
        /// 	"Thumbnail ResolutionY"
        /// 	"Thumbnail PlanarConfig"
        /// 	"Thumbnail ResolutionUnit"
        /// 	"Thumbnail TransferFunction"
        /// 	"Thumbnail SoftwareUsed"
        /// 	"Thumbnail DateTime"
        /// 	"Thumbnail Artist"
        /// 	"Thumbnail WhitePoint"
        /// 	"Thumbnail PrimaryChromaticities"
        /// 	"Thumbnail YCbCrCoefficients"
        /// 	"Thumbnail YCbCrSubsampling"
        /// 	"Thumbnail YCbCrPositioning"
        /// 	"Thumbnail RefBlackWhite"
        /// 	"Thumbnail CopyRight"
        /// 	"Luminance Table"
        /// 	"Chrominance Table"
        /// 	"Frame Delay"
        /// 	"Loop Count"
        /// 	"Pixel Unit"
        /// 	"Pixel PerUnit X"
        /// 	"Pixel PerUnit Y"
        /// 	"Palette Histogram"
        /// 	"Exposure Time"
        /// 	"F-Number"
        /// 	"Exposure Prog"
        /// 	"Spectral Sense"
        /// 	"ISO Speed"
        /// 	"OECF"
        /// 	"Ver"
        /// 	"DTOrig"
        /// 	"DTDigitized"
        /// 	"CompConfig"
        /// 	"CompBPP"
        /// 	"Shutter Speed"
        /// 	"Aperture"
        /// 	"Brightness"
        /// 	"Exposure Bias"
        /// 	"MaxAperture"
        /// 	"SubjectDist"
        /// 	"Metering Mode"
        /// 	"LightSource"
        /// 	"Flash"
        /// 	"FocalLength"
        /// 	"Maker Note"
        /// 	"User Comment"
        /// 	"DTSubsec"
        /// 	"DTOrigSS"
        /// 	"DTDigSS"
        /// 	"FPXVer"
        /// 	"ColorSpace"
        /// 	"PixXDim"
        /// 	"PixYDim"
        /// 	"RelatedWav"
        /// 	"Interop"
        /// 	"FlashEnergy"
        /// 	"SpatialFR"
        /// 	"FocalXRes"
        /// 	"FocalYRes"
        /// 	"FocalResUnit"
        /// 	"Subject Loc"
        /// 	"Exposure Index"
        /// 	"Sensing Method"
        /// 	"FileSource"
        /// 	"SceneType"
        /// 	"CfaPattern"
        /// 	"Gps Ver"
        /// 	"Gps LatitudeRef"
        /// 	"Gps Latitude"
        /// 	"Gps LongitudeRef"
        /// 	"Gps Longitude"
        /// 	"Gps AltitudeRef"
        /// 	"Gps Altitude"
        /// 	"Gps GpsTime"
        /// 	"Gps GpsSatellites"
        /// 	"Gps GpsStatus"
        /// 	"Gps GpsMeasureMode"
        /// 	"Gps GpsDop"
        /// 	"Gps SpeedRef"
        /// 	"Gps Speed"
        /// 	"Gps TrackRef"
        /// 	"Gps Track"
        /// 	"Gps ImgDirRef"
        /// 	"Gps ImgDir"
        /// 	"Gps MapDatum"
        /// 	"Gps DestLatRef"
        /// 	"Gps DestLat"
        /// 	"Gps DestLongRef"
        /// 	"Gps DestLong"
        /// 	"Gps DestBearRef"
        /// 	"Gps DestBear"
        /// 	"Gps DestDistRef"
        /// 	"Gps DestDist"
        /// </summary>
        public object this[string index]
        {
            get { return _properties[index]; }
        }

        internal int Count
        {
            get { return _properties.Count; }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return (new EXIFextractorEnumerator(_properties));
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "len"></param>
        /// <param name = "type"></param>
        /// <param name = "_data"></param>
        public void setTag(int id, string data)
        {
            Encoding ascii = Encoding.ASCII;
            setTag(id, data.Length, 0x2, ascii.GetBytes(data));
        }

        /// <summary>
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "len"></param>
        /// <param name = "type"></param>
        /// <param name = "_data"></param>
        public void setTag(int id, int len, short type, byte[] data)
        {
            PropertyItem p = CreatePropertyItem(type, id, len, data);
            _image.SetPropertyItem(p);
            buildDB(_image.PropertyItems);
        }

        /// <summary>
        /// </summary>
        /// <param name = "type"></param>
        /// <param name = "tag"></param>
        /// <param name = "len"></param>
        /// <param name = "value"></param>
        /// <returns></returns>
        private static PropertyItem CreatePropertyItem(short type, int tag, int len, byte[] value)
        {
            PropertyItem item;

            // Loads a PropertyItem from a Jpeg image stored in the assembly as a resource.
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream emptyBitmapStream = assembly.GetManifestResourceStream("EXIFExtractor.EXIFManifestHelper.jpg");
            Image empty = Image.FromStream(emptyBitmapStream);

            item = empty.PropertyItems[0];

            // Copies the _data to the property item.
            item.Type = type;
            item.Len = len;
            item.Id = tag;
            item.Value = new byte[value.Length];
            value.CopyTo(item.Value, 0);

            return item;
        }

        public static PropertyItem[] GetExifProperties(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            Image image = Image.FromStream(stream,
                                           /* useEmbeddedColorManagement = */ true,
                                           /* validateImageData = */ false);
            return image.PropertyItems;
        }

        /// <summary>
        /// </summary>
        private void buildDB(PropertyItem[] parr)
        {
            _properties.Clear();
            //
            _data = "";
            //
            Encoding ascii = Encoding.ASCII;
            //
            foreach (PropertyItem p in parr)
            {
                string v = "";
                string name = (string) _myHash[p.Id];
                // tag not found. skip it
                if (name == null) continue;
                //
                _data += name + ": ";
                //
                //1 = BYTE An 8-bit unsigned integer.,
                if (p.Type == 0x1)
                {
                    if (p.Value != null)
                    {
                        v = p.Value[0].ToString();
                    }
                    else
                    {
                        v = string.Empty;
                    }
                }
                    //2 = ASCII An 8-bit byte containing one 7-bit ASCII code. The final byte is terminated with NULL.,
                else if (p.Type == 0x2)
                {
                    // string					
                    v = ascii.GetString(p.Value);
                }
                    //3 = SHORT A 16-bit (2 -byte) unsigned integer,
                else if (p.Type == 0x3)
                {
                    // orientation // lookup table					
                    switch (p.Id)
                    {
                        case 0x8827: // ISO
                            v = "ISO-" + convertToInt16U(p.Value);
                            break;
                        case 0xA217: // sensing method
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 1:
                                        v = "Not defined";
                                        break;
                                    case 2:
                                        v = "One-chip color area sensor";
                                        break;
                                    case 3:
                                        v = "Two-chip color area sensor";
                                        break;
                                    case 4:
                                        v = "Three-chip color area sensor";
                                        break;
                                    case 5:
                                        v = "Color sequential area sensor";
                                        break;
                                    case 7:
                                        v = "Trilinear sensor";
                                        break;
                                    case 8:
                                        v = "Color sequential linear sensor";
                                        break;
                                    default:
                                        v = " reserved";
                                        break;
                                }
                            }
                            break;
                        case 0x8822: // aperture 
                            switch (convertToInt16U(p.Value))
                            {
                                case 0:
                                    v = "Not defined";
                                    break;
                                case 1:
                                    v = "Manual";
                                    break;
                                case 2:
                                    v = "Normal program";
                                    break;
                                case 3:
                                    v = "Aperture priority";
                                    break;
                                case 4:
                                    v = "Shutter priority";
                                    break;
                                case 5:
                                    v = "Creative program (biased toward depth of field)";
                                    break;
                                case 6:
                                    v = "Action program (biased toward fast shutter speed)";
                                    break;
                                case 7:
                                    v = "Portrait mode (for closeup photos with the background out of focus)";
                                    break;
                                case 8:
                                    v = "Landscape mode (for landscape photos with the background in focus)";
                                    break;
                                default:
                                    v = "reserved";
                                    break;
                            }
                            break;
                        case 0x9207: // metering mode
                            switch (convertToInt16U(p.Value))
                            {
                                case 0:
                                    v = "unknown";
                                    break;
                                case 1:
                                    v = "Average";
                                    break;
                                case 2:
                                    v = "CenterWeightedAverage";
                                    break;
                                case 3:
                                    v = "Spot";
                                    break;
                                case 4:
                                    v = "MultiSpot";
                                    break;
                                case 5:
                                    v = "Pattern";
                                    break;
                                case 6:
                                    v = "Partial";
                                    break;
                                case 255:
                                    v = "Other";
                                    break;
                                default:
                                    v = "reserved";
                                    break;
                            }
                            break;
                        case 0x9208: // light source
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 0:
                                        v = "unknown";
                                        break;
                                    case 1:
                                        v = "Daylight";
                                        break;
                                    case 2:
                                        v = "Fluorescent";
                                        break;
                                    case 3:
                                        v = "Tungsten";
                                        break;
                                    case 17:
                                        v = "Standard light A";
                                        break;
                                    case 18:
                                        v = "Standard light B";
                                        break;
                                    case 19:
                                        v = "Standard light C";
                                        break;
                                    case 20:
                                        v = "D55";
                                        break;
                                    case 21:
                                        v = "D65";
                                        break;
                                    case 22:
                                        v = "D75";
                                        break;
                                    case 255:
                                        v = "other";
                                        break;
                                    default:
                                        v = "reserved";
                                        break;
                                }
                            }
                            break;
                        case 0x9209:
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 0:
                                        v = "Flash did not fire";
                                        break;
                                    case 1:
                                        v = "Flash fired";
                                        break;
                                    case 5:
                                        v = "Strobe return light not detected";
                                        break;
                                    case 7:
                                        v = "Strobe return light detected";
                                        break;
                                    default:
                                        v = "reserved";
                                        break;
                                }
                            }
                            break;
                        default:
                            v = convertToInt16U(p.Value).ToString();
                            break;
                    }
                }
                    //4 = LONG A 32-bit (4 -byte) unsigned integer,
                else if (p.Type == 0x4)
                {
                    // orientation // lookup table					
                    v = convertToInt32U(p.Value).ToString();
                }
                    //5 = RATIONAL Two LONGs. The first LONG is the numerator and the second LONG expresses the//denominator.,
                else if (p.Type == 0x5)
                {
                    // rational
                    byte[] n = new byte[p.Len/2];
                    byte[] d = new byte[p.Len/2];
                    Array.Copy(p.Value, 0, n, 0, p.Len/2);
                    Array.Copy(p.Value, p.Len/2, d, 0, p.Len/2);
                    uint a = convertToInt32U(n);
                    uint b = convertToInt32U(d);
                    Rational r = new Rational(a, b);
                    //
                    //convert here
                    //
                    switch (p.Id)
                    {
                        case 0x9202: // aperture
                            v = "F/" + Math.Round(Math.Pow(Math.Sqrt(2), r.ToDouble()), 2);
                            break;
                        case 0x920A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829D: // F-number
                            v = "F/" + r.ToDouble();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }
                }
                    //7 = UNDEFINED An 8-bit byte that can take any value depending on the field definition,
                else if (p.Type == 0x7)
                {
                    switch (p.Id)
                    {
                        case 0xA300:
                            {
                                if (p.Value[0] == 3)
                                {
                                    v = "DSC";
                                }
                                else
                                {
                                    v = "reserved";
                                }
                                break;
                            }
                        case 0xA301:
                            if (p.Value[0] == 1)
                                v = "A directly photographed image";
                            else
                                v = "Not a directly photographed image";
                            break;
                        default:
                            v = "-";
                            break;
                    }
                }				
                    //9 = SLONG A 32-bit (4 -byte) signed integer (2's complement notation),
                else if (p.Type == 0x9)
                {
                    v = convertToInt32(p.Value).ToString();
                }
                    //10 = SRATIONAL Two SLONGs. The first SLONG is the numerator and the second SLONG is the
                    //denominator.
                else if (p.Type == 0xA)
                {
                    // rational
                    byte[] n = new byte[p.Len/2];
                    byte[] d = new byte[p.Len/2];
                    Array.Copy(p.Value, 0, n, 0, p.Len/2);
                    Array.Copy(p.Value, p.Len/2, d, 0, p.Len/2);
                    int a = convertToInt32(n);
                    int b = convertToInt32(d);
                    Rational r = new Rational(a, b);
                    //
                    // convert here
                    //
                    switch (p.Id)
                    {
                        case 0x9201: // shutter speed
                            v = "1/" + Math.Round(Math.Pow(2, r.ToDouble()), 2);
                            break;
                        case 0x9203:
                            v = Math.Round(r.ToDouble(), 4).ToString();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }
                }
                // add it to the list
                if (_properties[name] == null)
                    _properties.Add(name, v);
                // cat it too
                _data += v;
                _data += sp;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _data;
        }

        /// <summary>
        /// </summary>
        /// <param name = "arr"></param>
        /// <returns></returns>
        private int convertToInt32(byte[] arr)
        {
            if (arr.Length != 4)
                return 0;
            else
                return arr[3] << 24 | arr[2] << 16 | arr[1] << 8 | arr[0];
        }

        /// <summary>
        /// </summary>
        /// <param name = "arr"></param>
        /// <returns></returns>
        private int convertToInt16(byte[] arr)
        {
            if (arr.Length != 2)
                return 0;
            else
                return arr[1] << 8 | arr[0];
        }

        /// <summary>
        /// </summary>
        /// <param name = "arr"></param>
        /// <returns></returns>
        private uint convertToInt32U(byte[] arr)
        {
            if (arr.Length != 4)
                return 0;
            else
                return Convert.ToUInt32(arr[3] << 24 | arr[2] << 16 | arr[1] << 8 | arr[0]);
        }

        /// <summary>
        /// </summary>
        /// <param name = "arr"></param>
        /// <returns></returns>
        private uint convertToInt16U(byte[] arr)
        {
            if (arr.Length != 2)
                return 0;
            else
                return Convert.ToUInt16(arr[1] << 8 | arr[0]);
        }
    }

    //
    // dont touch this class. its for IEnumerator
    // 
    //
    internal class EXIFextractorEnumerator : IEnumerator
    {
        private Hashtable exifTable;
        private IDictionaryEnumerator index;

        internal EXIFextractorEnumerator(Hashtable exif)
        {
            exifTable = exif;
            Reset();
            index = exif.GetEnumerator();
        }

        #region IEnumerator Members

        public void Reset()
        {
            index = null;
        }

        public object Current
        {
            get { return (new Pair(index.Key, index.Value)); }
        }

        public bool MoveNext()
        {
            if (index != null && index.MoveNext())
                return true;
            else
                return false;
        }

        #endregion
    }
}