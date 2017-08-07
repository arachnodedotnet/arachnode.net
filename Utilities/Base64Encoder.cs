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

#region region

#endregion

namespace Arachnode.Utilities
{
    public class Base64Encoder
    {
        private readonly int _blockCount;
        private readonly int _length;
        private readonly int _length2;
        private readonly int _paddingCount;
        private readonly byte[] _source;

        public Base64Encoder(byte[] input)
        {
            _source = input;
            _length = input.Length;
            if ((_length%3) == 0)
            {
                _paddingCount = 0;
                _blockCount = _length/3;
            }
            else
            {
                _paddingCount = 3 - (_length%3); //need to add padding
                _blockCount = (_length + _paddingCount)/3;
            }
            _length2 = _length + _paddingCount; //or blockCount *3
        }

        public char[] GetEncoded()
        {
            byte[] source2;
            source2 = new byte[_length2];
            //copy data over insert padding
            for (int x = 0; x < _length2; x++)
            {
                if (x < _length)
                {
                    source2[x] = _source[x];
                }
                else
                {
                    source2[x] = 0;
                }
            }

            byte b1, b2, b3;
            byte temp, temp1, temp2, temp3, temp4;
            var buffer = new byte[_blockCount*4];
            var result = new char[_blockCount*4];
            for (int x = 0; x < _blockCount; x++)
            {
                b1 = source2[x*3];
                b2 = source2[x*3 + 1];
                b3 = source2[x*3 + 2];

                temp1 = (byte) ((b1 & 252) >> 2); //first

                temp = (byte) ((b1 & 3) << 4);
                temp2 = (byte) ((b2 & 240) >> 4);
                temp2 += temp; //second

                temp = (byte) ((b2 & 15) << 2);
                temp3 = (byte) ((b3 & 192) >> 6);
                temp3 += temp; //third

                temp4 = (byte) (b3 & 63); //fourth

                buffer[x*4] = temp1;
                buffer[x*4 + 1] = temp2;
                buffer[x*4 + 2] = temp3;
                buffer[x*4 + 3] = temp4;
            }

            for (int x = 0; x < _blockCount*4; x++)
            {
                result[x] = sixbit2char(buffer[x]);
            }

            //covert last "A"s to "=", based on paddingCount
            switch (_paddingCount)
            {
                case 0:
                    break;
                case 1:
                    result[_blockCount*4 - 1] = '=';
                    break;
                case 2:
                    result[_blockCount*4 - 1] = '=';
                    result[_blockCount*4 - 2] = '=';
                    break;
                default:
                    break;
            }
            return result;
        }

        private char sixbit2char(byte b)
        {
            var lookupTable = new char[64]
                                  {
                                      'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                                      'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                                      '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
                                  };

            if ((b >= 0) && (b <= 63))
            {
                return lookupTable[b];
            }
            else
            {
                //should not happen;
                return ' ';
            }
        }
    }
}