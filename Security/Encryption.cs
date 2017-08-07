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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

#endregion

namespace Arachnode.Security
{
    public static class Encryption
    {
        private static readonly byte[] _iv_RijndaelManaged = {91, 23, 65, 114, 213, 26, 117, 118, 90, 32, 2, 3, 43, 66, 34, 5};
        private static readonly byte[] _iv_TripleDES = {91, 23, 65, 114, 213, 26, 117, 118};
        private static readonly byte[] _key_RijndaelManaged = {113, 23, 98, 231, 213, 17, 172, 3, 76, 183, 23, 217, 231, 29, 99, 108, 2, 56, 7, 80, 97, 8, 74, 66, 65, 43, 23, 54, 33, 31, 32, 30};
        private static readonly byte[] _key_TripleDES = {113, 23, 98, 231, 213, 17, 172, 3, 76, 183, 23, 217, 231, 29, 99, 108};

        /// <summary>
        /// 	Encrypts the triple DES.
        /// </summary>
        /// <param name = "toBeEncrypted">To be encrypted.</param>
        /// <returns></returns>
        public static string EncryptTripleDES(string toBeEncrypted)
        {
            return EncryptTripleDES(toBeEncrypted, _key_TripleDES, _iv_TripleDES);
        }

        /// <summary>
        /// 	Encrypts the triple DES.
        /// </summary>
        /// <param name = "toBeEncrypted">To be encrypted.</param>
        /// <param name = "key">The key.</param>
        /// <param name = "iv">The iv.</param>
        /// <returns></returns>
        public static string EncryptTripleDES(string toBeEncrypted, byte[] key, byte[] iv)
        {
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();

            byte[] inputBytes = Encoding.UTF8.GetBytes(toBeEncrypted);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateEncryptor(key, iv), CryptoStreamMode.Write);

            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();

            return HttpUtility.UrlEncode(Convert.ToBase64String(memoryStream.ToArray()).Replace("+", "@"));
        }

        /// <summary>
        /// 	Decrypts the triple DES.
        /// </summary>
        /// <param name = "toBeDecrypted">To be decrypted.</param>
        /// <returns></returns>
        public static string DecryptTripleDES(string toBeDecrypted)
        {
            return DecryptTripleDES(toBeDecrypted, _key_TripleDES, _iv_TripleDES);
        }

        /// <summary>
        /// 	Decrypts the triple DES.
        /// </summary>
        /// <param name = "toBeDecrypted">To be decrypted.</param>
        /// <param name = "key">The key.</param>
        /// <param name = "iv">The iv.</param>
        /// <returns></returns>
        public static string DecryptTripleDES(string toBeDecrypted, byte[] key, byte[] iv)
        {
            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();

            byte[] inputBytes = Convert.FromBase64String(HttpUtility.UrlDecode(toBeDecrypted).Replace("@", "+"));

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateDecryptor(key, iv), CryptoStreamMode.Write);

            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        /// <summary>
        /// 	Encrypts the rijndael managed.
        /// </summary>
        /// <param name = "toBeEncrypted">To be encrypted.</param>
        /// <returns></returns>
        public static string EncryptRijndaelManaged(string toBeEncrypted)
        {
            return EncryptRijndaelManaged(toBeEncrypted, _key_RijndaelManaged, _iv_RijndaelManaged);
        }

        /// <summary>
        /// 	Encrypts the rijndael managed.
        /// </summary>
        /// <param name = "toBeEncrypted">To be encrypted.</param>
        /// <param name = "key">The key.</param>
        /// <param name = "iv">The iv.</param>
        /// <returns></returns>
        public static string EncryptRijndaelManaged(string toBeEncrypted, byte[] key, byte[] iv)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            byte[] inputBytes = Encoding.UTF8.GetBytes(toBeEncrypted);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateEncryptor(key, iv), CryptoStreamMode.Write);

            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();

            return HttpUtility.UrlEncode(Convert.ToBase64String(memoryStream.ToArray()).Replace("+", "@"));
        }

        /// <summary>
        /// 	Decrypts the rijndael managed.
        /// </summary>
        /// <param name = "toBeDecrypted">To be decrypted.</param>
        /// <returns></returns>
        public static string DecryptRijndaelManaged(string toBeDecrypted)
        {
            return DecryptRijndaelManaged(toBeDecrypted, _key_RijndaelManaged, _iv_RijndaelManaged);
        }

        /// <summary>
        /// 	Decrypts the rijndael managed.
        /// </summary>
        /// <param name = "toBeDecrypted">To be decrypted.</param>
        /// <param name = "key">The key.</param>
        /// <param name = "iv">The iv.</param>
        /// <returns></returns>
        public static string DecryptRijndaelManaged(string toBeDecrypted, byte[] key, byte[] iv)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            byte[] inputBytes = Convert.FromBase64String(HttpUtility.UrlDecode(toBeDecrypted).Replace("@", "+"));

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(key, iv), CryptoStreamMode.Write);

            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}