using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataAccess.Value.Interfaces;
using Arachnode.SiteCrawler.Managers;
using Arachnode.SiteCrawler.Value;
using Arachnode.SiteCrawler.Value.AbstractClasses;

namespace Arachnode.Plugins.CrawlRules
{
    internal class HtmlSlimmer<TArachnodeDAO> : ACrawlRule<TArachnodeDAO> where TArachnodeDAO : IArachnodeDAO
    {
        // Match characters
        private static Regex MultiWhiteSpace = new Regex("\\s{2,}", RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex NonPrintableChars = new Regex("[\\x00-\\x09]|[\\x10-\\x19]|[\\x0b-\\x0c]|[\\x0e-\\x0f]|[\\x1b-\\x1f]", RegexOptions.Compiled);

        // Match tags and their contents
        private static Regex HeadTag = new Regex("<\\s*head[^>]*>([\\w\\W]*?)<\\s*/head\\s*>|<\\s*head[^>]*>|<\\s*/head\\s*>", RegexOptions.IgnoreCase ^ RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex ScriptTag = new Regex("<\\s*script[^>]*>([\\w\\W]*?)<\\s*/script\\s*>", RegexOptions.IgnoreCase ^ RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex StyleTag = new Regex("<\\s*style[^>]*>([\\w\\W]*?)<\\s*/style\\s*>", RegexOptions.IgnoreCase ^ RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex LinkTag = new Regex("<\\s*link[^>]*>([\\w\\W]*?)<\\s*/link\\s*>", RegexOptions.IgnoreCase ^ RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex CommentsTag = new Regex("<!--[^-]*--\\s*>", RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex IFrameTag = new Regex("<\\s*iframe[^>]*>([\\w\\W]*?)<\\s*/iframe\\s*>", RegexOptions.IgnoreCase ^ RegexOptions.Multiline ^ RegexOptions.Compiled);
        private static Regex MetaTag = new Regex("<\\s*meta[^>]*>([\\w\\W]*?)<\\s*/meta\\s*>", RegexOptions.IgnoreCase ^ RegexOptions.Multiline ^ RegexOptions.Compiled);

        private bool _lzfCompress;
        private bool _removeCarriageReturns;
        private bool _removeNewLines;
        private bool _removeMultiWhiteSpaces;
        private bool _removeNonPrintableChars;
        private bool _removeHeadTags;
        private bool _removeScriptTags;
        private bool _removeStyleTags;
        private bool _removeLinkTags;
        private bool _removeCommentsTags;
        private bool _removeIFrameTags;
        private bool _removeMetaTags;

        #region Public Methods

        public HtmlSlimmer(ApplicationSettings applicationSettings, WebSettings webSettings)
            : base(applicationSettings, webSettings)
        {
        }

        /// <summary>
        /// 	Assigns the additional parameters.
        /// </summary>
        /// <param name = "settings"></param>
        public override void AssignSettings(Dictionary<string, string> settings)
        {
            if (settings.ContainsKey("LZFCompress"))
                _lzfCompress = bool.Parse(settings["LZFCompress"]);
            if (settings.ContainsKey("RemoveCarriageReturns"))
                _removeCarriageReturns = bool.Parse(settings["RemoveCarriageReturns"]);
            if (settings.ContainsKey("RemoveNewLines"))
                _removeNewLines = bool.Parse(settings["RemoveNewLines"]);
            if (settings.ContainsKey("RemoveMultiWhiteSpaces"))
                _removeMultiWhiteSpaces = bool.Parse(settings["RemoveMultiWhiteSpaces"]);
            if (settings.ContainsKey("RemoveNonPrintableChars"))
                _removeNonPrintableChars = bool.Parse(settings["RemoveNonPrintableChars"]);
            if (settings.ContainsKey("RemoveHeadTags"))
                _removeHeadTags = bool.Parse(settings["RemoveHeadTags"]);
            if (settings.ContainsKey("RemoveScriptTags"))
                _removeScriptTags = bool.Parse(settings["RemoveScriptTags"]);
            if (settings.ContainsKey("RemoveStyleTags"))
                _removeStyleTags = bool.Parse(settings["RemoveStyleTags"]);
            if (settings.ContainsKey("RemoveCommentsTags"))
                _removeCommentsTags = bool.Parse(settings["RemoveCommentsTags"]);
            if (settings.ContainsKey("RemoveLinkTags"))
                _removeLinkTags = bool.Parse(settings["RemoveLinkTags"]);
            if (settings.ContainsKey("RemoveIFrameTags"))
                _removeIFrameTags = bool.Parse(settings["RemoveIFrameTags"]);
            if (settings.ContainsKey("RemoveMetaTags"))
                _removeMetaTags = bool.Parse(settings["RemoveMetaTags"]);
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "crawlRequest">The crawl request.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(CrawlRequest<TArachnodeDAO> crawlRequest, IArachnodeDAO arachnodeDAO)
        {
            if (string.IsNullOrEmpty(crawlRequest.Html)) return false;

            var html = crawlRequest.Html;
            var lzf = new LZF();

            if (_removeCarriageReturns)
                html = Replace(html, "\r", "", StringComparison.CurrentCulture);
            if (_removeNewLines)
                html = Replace(html, "\n", "", StringComparison.CurrentCulture);
            if (_removeMultiWhiteSpaces)
                html = MultiWhiteSpace.Replace(html, "");
            if (_removeNonPrintableChars)
                html = NonPrintableChars.Replace(html, "");
            if (_removeHeadTags)
                html = HeadTag.Replace(html, "");
            if (_removeScriptTags)
                html = ScriptTag.Replace(html, "");
            if (_removeStyleTags)
                html = StyleTag.Replace(html, "");
            if (_removeCommentsTags)
                html = CommentsTag.Replace(html, "");
            if (_removeLinkTags)
                html = LinkTag.Replace(html, "");
            if (_removeIFrameTags)
                html = IFrameTag.Replace(html, "");
            if (_removeMetaTags)
                html = MetaTag.Replace(html, "");

            if (_lzfCompress)
            {
                var data = System.Text.Encoding.UTF8.GetBytes(html);
                var destination = new Byte[data.Length + 36000];
                int size = 0;

                size = lzf.Compress(data, data.Length, destination, destination.Length);

                if (size > 0)
                {
                    var compressed = new byte[size];
                    for (UInt32 i = 0; i < size; ++i) compressed[i] = destination[i];
                    crawlRequest.Data = compressed;
                }
            }
            else
            {
                crawlRequest.Data = System.Text.Encoding.UTF8.GetBytes(html);
            }

            crawlRequest.Html = html;
            crawlRequest.DecodedHtml = System.Web.HttpUtility.HtmlDecode(html);

            return false;
        }

        /// <summary>
        /// 	Determines whether the specified crawl request is disallowed.
        /// </summary>
        /// <param name = "discovery">The discovery.</param>
        /// <param name = "arachnodeDAO">The arachnode DAO.</param>
        /// <returns>
        /// 	<c>true</c> if the specified crawl request is disallowed; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsDisallowed(Discovery<TArachnodeDAO> discovery, IArachnodeDAO arachnodeDAO)
        {
            return false;
        }

        /// <summary>
        /// 	Stops this instance.
        /// </summary>
        public override void Stop()
        {
        }

        /// <summary>
        /// 	Decompresses a LZF compressed byte array to a string.
        /// </summary>
        /// <param name = "compressedHtml">LZF compressed data.</param>
        /// <returns>
        /// 	Decompressed string data.
        /// </returns>
        public static string DecompressLZF(byte[] compressedHtml)
        {
            var lzf = new LZF();
            var decompressedHtml = new byte[compressedHtml.Length * 10];
            var size = lzf.Decompress(compressedHtml, compressedHtml.Length, decompressedHtml, decompressedHtml.Length);
            Array.Resize(ref decompressedHtml, size);
            return System.Text.Encoding.UTF8.GetString(decompressedHtml);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 	Fast string replace. Faster than String.Replace.
        /// </summary>
        private static string Replace(string original, string pattern, string replacement, StringComparison comparisonType)
        {
            return Replace(original, pattern, replacement, comparisonType, -1);
        }

        /// <summary>
        /// 	Fast string replace. Faster than String.Replace.
        /// </summary>
        private static string Replace(string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize)
        {
            if (String.IsNullOrEmpty(original) || String.IsNullOrEmpty(pattern))
                return original;
            
            int posCurrent = 0;
            int lenPattern = pattern.Length;
            int idxNext = original.IndexOf(pattern, comparisonType);
            var result = new StringBuilder(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

            while (idxNext >= 0)
            {
                result.Append(original, posCurrent, idxNext - posCurrent);
                result.Append(replacement);

                posCurrent = idxNext + lenPattern;

                idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
            }

            result.Append(original, posCurrent, original.Length - posCurrent);

            return result.ToString();
        }

        #endregion
    }

    #region Nested Class LZF

    /*
     * Improved version to C# LibLZF Port:
     * Copyright (c) 2010 Roman Atachiants <kelindar@gmail.com>
     * 
     * Original CLZF Port:
     * Copyright (c) 2005 Oren J. Maurice <oymaurice@hazorea.org.il>
     * 
     * Original LibLZF Library & Algorithm:
     * Copyright (c) 2000-2008 Marc Alexander Lehmann <schmorp@schmorp.de>
     * 
     * Redistribution and use in source and binary forms, with or without modifica-
     * tion, are permitted provided that the following conditions are met:
     * 
     *   1.  Redistributions of source code must retain the above copyright notice,
     *       this list of conditions and the following disclaimer.
     * 
     *   2.  Redistributions in binary form must reproduce the above copyright
     *       notice, this list of conditions and the following disclaimer in the
     *       documentation and/or other materials provided with the distribution.
     * 
     *   3.  The name of the author may not be used to endorse or promote products
     *       derived from this software without specific prior written permission.
     * 
     * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
     * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MER-
     * CHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO
     * EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPE-
     * CIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
     * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
     * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
     * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTH-
     * ERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
     * OF THE POSSIBILITY OF SUCH DAMAGE.
     *
     * Alternatively, the contents of this file may be used under the terms of
     * the GNU General Public License version 2 (the "GPL"), in which case the
     * provisions of the GPL are applicable instead of the above. If you wish to
     * allow the use of your version of this file only under the terms of the
     * GPL and not to allow others to use your version of this file under the
     * BSD license, indicate your decision by deleting the provisions above and
     * replace them with the notice and other provisions required by the GPL. If
     * you do not delete the provisions above, a recipient may use your version
     * of this file under either the BSD or the GPL.
     */

    /* Benchmark with Alice29 Canterbury Corpus
        ---------------------------------------
        (Compression) Original CLZF C#
        Raw = 152089, Compressed = 101092
            8292,4743 ms.
        ---------------------------------------
        (Compression) My LZF C#
        Raw = 152089, Compressed = 101092
            33,0019 ms.
        ---------------------------------------
        (Compression) Zlib using SharpZipLib
        Raw = 152089, Compressed = 54388
            8389,4799 ms.
        ---------------------------------------
        (Compression) QuickLZ C#
        Raw = 152089, Compressed = 83494
            80,0046 ms.
        ---------------------------------------
        (Decompression) Original CLZF C#
        Decompressed = 152089
            16,0009 ms.
        ---------------------------------------
        (Decompression) My LZF C#
        Decompressed = 152089
            15,0009 ms.
        ---------------------------------------
        (Decompression) Zlib using SharpZipLib
        Decompressed = 152089
            3577,2046 ms.
        ---------------------------------------
        (Decompression) QuickLZ C#
        Decompressed = 152089
            21,0012 ms.
    */


    /// <summary>
    /// Improved C# LZF Compressor, a very small data compression library. The compression algorithm is extremely fast. 

    /// </summary>
    public sealed class LZF
    {
        /// <summary>
        /// Hashtable, thac can be allocated only once
        /// </summary>
        private readonly long[] HashTable = new long[HSIZE];

        private const uint HLOG = 14;
        private const uint HSIZE = (1 << 14);
        private const uint MAX_LIT = (1 << 5);
        private const uint MAX_OFF = (1 << 13);
        private const uint MAX_REF = ((1 << 8) + (1 << 3));

        /// <summary>
        /// Compresses the data using LibLZF algorithm
        /// </summary>
        /// <param name="input">Reference to the data to compress</param>
        /// <param name="inputLength">Lenght of the data to compress</param>
        /// <param name="output">Reference to a buffer which will contain the compressed data</param>
        /// <param name="outputLength">Lenght of the compression buffer (should be bigger than the input buffer)</param>
        /// <returns>The size of the compressed archive in the output buffer</returns>
        public int Compress(byte[] input, int inputLength, byte[] output, int outputLength)
        {
            Array.Clear(HashTable, 0, (int)HSIZE);

            long hslot;
            uint iidx = 0;
            uint oidx = 0;
            long reference;

            uint hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]); // FRST(in_data, iidx);
            long off;
            int lit = 0;

            for (; ; )
            {
                if (iidx < inputLength - 2)
                {
                    hval = (hval << 8) | input[iidx + 2];
                    hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
                    reference = HashTable[hslot];
                    HashTable[hslot] = (long)iidx;


                    if ((off = iidx - reference - 1) < MAX_OFF
                        && iidx + 4 < inputLength
                        && reference > 0
                        && input[reference + 0] == input[iidx + 0]
                        && input[reference + 1] == input[iidx + 1]
                        && input[reference + 2] == input[iidx + 2]
                        )
                    {
                        /* match found at *reference++ */
                        uint len = 2;
                        uint maxlen = (uint)inputLength - iidx - len;
                        maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;

                        if (oidx + lit + 1 + 3 >= outputLength)
                            return 0;

                        do
                            len++;
                        while (len < maxlen && input[reference + len] == input[iidx + len]);

                        if (lit != 0)
                        {
                            output[oidx++] = (byte)(lit - 1);
                            lit = -lit;
                            do
                                output[oidx++] = input[iidx + lit];
                            while ((++lit) != 0);
                        }

                        len -= 2;
                        iidx++;

                        if (len < 7)
                        {
                            output[oidx++] = (byte)((off >> 8) + (len << 5));
                        }
                        else
                        {
                            output[oidx++] = (byte)((off >> 8) + (7 << 5));
                            output[oidx++] = (byte)(len - 7);
                        }

                        output[oidx++] = (byte)off;

                        iidx += len - 1;
                        hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]); 

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;

                        hval = (hval << 8) | input[iidx + 2];
                        HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
                        iidx++;
                        continue;
                    }
                }
                else if (iidx == inputLength)
                    break;

                /* one more literal byte we must copy */
                lit++;
                iidx++;

                if (lit == MAX_LIT)
                {
                    if (oidx + 1 + MAX_LIT >= outputLength)
                        return 0;

                    output[oidx++] = (byte)(MAX_LIT - 1);
                    lit = -lit;
                    do
                        output[oidx++] = input[iidx + lit];
                    while ((++lit) != 0);
                }
            }

            if (lit != 0)
            {
                if (oidx + lit + 1 >= outputLength)
                    return 0;

                output[oidx++] = (byte)(lit - 1);
                lit = -lit;
                do
                    output[oidx++] = input[iidx + lit];
                while ((++lit) != 0);
            }

            return (int)oidx;
        }


        /// <summary>
        /// Decompresses the data using LibLZF algorithm
        /// </summary>
        /// <param name="input">Reference to the data to decompress</param>
        /// <param name="inputLength">Lenght of the data to decompress</param>
        /// <param name="output">Reference to a buffer which will contain the decompressed data</param>
        /// <param name="outputLength">The size of the decompressed archive in the output buffer</param>
        /// <returns>Returns decompressed size</returns>
        public int Decompress(byte[] input, int inputLength, byte[] output, int outputLength)
        {
            uint iidx = 0;
            uint oidx = 0;

            do
            {
                uint ctrl = input[iidx++];

                if (ctrl < (1 << 5)) /* literal run */
                {
                    ctrl++;

                    if (oidx + ctrl > outputLength)
                    {
                        //SET_ERRNO (E2BIG);
                        return 0;
                    }

                    do
                        output[oidx++] = input[iidx++];
                    while ((--ctrl) != 0);
                }
                else /* back reference */
                {
                    uint len = ctrl >> 5;

                    int reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);

                    if (len == 7)
                        len += input[iidx++];

                    reference -= input[iidx++];

                    if (oidx + len + 2 > outputLength)
                    {
                        //SET_ERRNO (E2BIG);
                        return 0;
                    }

                    if (reference < 0)
                    {
                        //SET_ERRNO (EINVAL);
                        return 0;
                    }

                    output[oidx++] = output[reference++];
                    output[oidx++] = output[reference++];

                    do
                        output[oidx++] = output[reference++];
                    while ((--len) != 0);
                }
            }
            while (iidx < inputLength);

            return (int)oidx;
        }
    }

    #endregion
}