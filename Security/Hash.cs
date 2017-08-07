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
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace Arachnode.Security
{
    public sealed class Hash
    {
        private MD5 _md5;
        private byte[] _hash;

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Hash" /> class.
        /// </summary>
        public Hash()
        {
            _md5 = MD5.Create();   
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Hash" /> class.
        /// </summary>
        /// <param name = "source">The source.</param>
        public Hash(byte[] source) : this()
        {
            _hash = _md5.ComputeHash(source);
        }

        /// <summary>
        /// 	Initializes a new instance of the <see cref = "Hash" /> class.
        /// </summary>
        /// <param name = "source">The source.</param>
        public Hash(string source) : this(Encoding.UTF8.GetBytes(source))
        {
        }

        /// <summary>
        /// 	Gets the <see cref = "System.Byte" /> at the specified index.
        /// </summary>
        /// <value></value>
        public byte this[int index]
        {
            get { return _hash[index]; }
        }

        public string HashBytesToString(byte[] source)
        {
            _hash = _md5.ComputeHash(source);

            return ToString();
        }

        public long HashBytesToLong(byte[] source)
        {
            return long.Parse(HashBytesToString(source));
        }

        public string HashStringToString(string source)
        {
            _hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(source));

            return source.GetHashCode().ToString();
            //return ToString();
        }

        public long HashStringToLong(string source)
        {
            return long.Parse(HashStringToString(source));
        }

        /// <summary>
        /// 	Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// 	A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(_hash.Length);

            for (int i = 0; i < _hash.Length; i++)
            {
                stringBuilder.Append(_hash[i]);
            }

            return stringBuilder.ToString();
        }
    }
}