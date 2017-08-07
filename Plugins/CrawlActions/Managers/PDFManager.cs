#region License : arachnode.net

// Copyright (c) 2015 http://arachnode.net, arachnode.net, LLC
//  
// Permission is hereby granted, upon purchase, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, merge and modify copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// LICENSE (ALL VERSIONS/EDITIONS): http://arachnode.net/r.ashx?3
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

#endregion

namespace Arachnode.Plugins.CrawlActions.Managers
{
    /// <summary>
    /// 	Parses a PDF file and extracts the text from it.
    /// </summary>
    public class PDFManager
    {
        #region Fields

        #region _numberOfCharsToKeep

        /// <summary>
        /// 	The number of characters to keep, when extracting text.
        /// </summary>
        private static int _numberOfCharsToKeep = 15;

        #endregion

        #endregion

        #region ExtractText

        /// <summary>
        /// 	Extracts a text from a PDF file.
        /// </summary>
        /// <param name = "inFileName">the full path to the pdf file.</param>
        /// <param name = "outFileName">the output file name.</param>
        /// <returns>the extracted text</returns>
        public bool ExtractText(string inFileName, string outFileName)
        {
            StreamWriter outFile = null;
            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(inFileName);
                //outFile = File.CreateText(outFileName);
                outFile = new StreamWriter(outFileName, false, Encoding.UTF8);

                Console.Write("Processing: ");

                int totalLen = 68;
                float charUnit = (totalLen)/(float) reader.NumberOfPages;
                int totalWritten = 0;
                float curUnit = 0;

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    outFile.Write(ExtractText(reader.GetPageContent(page)) + " ");

                    // Write the progress.
                    if (charUnit >= 1.0f)
                    {
                        for (int i = 0; i < (int) charUnit; i++)
                        {
                            Console.Write("#");
                            totalWritten++;
                        }
                    }
                    else
                    {
                        curUnit += charUnit;
                        if (curUnit >= 1.0f)
                        {
                            for (int i = 0; i < (int) curUnit; i++)
                            {
                                Console.Write("#");
                                totalWritten++;
                            }
                            curUnit = 0;
                        }
                    }
                }

                if (totalWritten < totalLen)
                {
                    for (int i = 0; i < (totalLen - totalWritten); i++)
                    {
                        Console.Write("#");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (outFile != null)
                {
                    outFile.Close();
                }
            }
        }

        #endregion

        #region ExtractText

        /// <summary>
        /// 	This method processes an uncompressed Adobe (text) object 
        /// 	and extracts text.
        /// </summary>
        /// <param name = "input">uncompressed</param>
        /// <returns></returns>
        public string ExtractText(byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                return "";
            }

            try
            {
                string resultString = "";

                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;

                // Flag showing if the next character is literal 
                // e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;

                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;

                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[_numberOfCharsToKeep];
                for (int j = 0; j < _numberOfCharsToKeep; j++)
                {
                    previousCharacters[j] = ' ';
                }

                for (int i = 0; i < input.Length; i++)
                {
                    char c = (char) input[i];

                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new[] {"TD", "Td"}, previousCharacters))
                            {
                                if (!resultString.EndsWith("\n\r"))
                                {
                                    resultString += "\n\r";
                                }
                            }
                            else
                            {
                                if (CheckToken(new[] {"'", "T*", "\""}, previousCharacters))
                                {
                                    if (!resultString.EndsWith("\n"))
                                    {
                                        resultString += "\n";
                                    }
                                }
                                else
                                {
                                    if (CheckToken(new[] {"Tj"}, previousCharacters))
                                    {
                                        if (!resultString.EndsWith(" "))
                                        {
                                            resultString += " ";
                                        }
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(new[] {"ET"}, previousCharacters))
                        {
                            inTextObject = false;
                            if (!resultString.EndsWith(" "))
                            {
                                resultString += " ";
                            }
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. 
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                                resultString += c.ToString();
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for 
                    // when we have to go back for a checking
                    for (int j = 0; j < _numberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[_numberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new[] {"BT"}, previousCharacters))
                    {
                        inTextObject = true;
                    }
                }
                return resultString;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region CheckToken

        /// <summary>
        /// 	Check if a certain 2 character token just came along (e.g. BT)
        /// </summary>
        /// <param name = "tokens">the searched token</param>
        /// <param name = "recent">the recent character array</param>
        /// <returns></returns>
        private bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    if (token.Length > 1)
                    {
                        if ((recent[_numberOfCharsToKeep - 3] == token[0]) &&
                            (recent[_numberOfCharsToKeep - 2] == token[1]) &&
                            ((recent[_numberOfCharsToKeep - 1] == ' ') ||
                             (recent[_numberOfCharsToKeep - 1] == 0x0d) ||
                             (recent[_numberOfCharsToKeep - 1] == 0x0a)) &&
                            ((recent[_numberOfCharsToKeep - 4] == ' ') ||
                             (recent[_numberOfCharsToKeep - 4] == 0x0d) ||
                             (recent[_numberOfCharsToKeep - 4] == 0x0a))
                            )
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        /// BT = Beginning of a text object operator 
        /// ET = End of a text object operator
        /// Td move to the start of next line
        ///  5 Ts = superscript
        /// -5 Ts = subscript
        /// 
        
        public List<byte[]> ExtractImages(string inFileName, ImageFormat imageFormat, int minimumHeight, int minumumWidth)
        {
            List<byte[]> extractedImages = new List<byte[]>();

            PdfReader pdfReader = new PdfReader(inFileName);

            for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
            {
                PdfDictionary pdfDictionary = pdfReader.GetPageN(pageNumber);
                PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pdfDictionary.Get(PdfName.RESOURCES));
                PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);

                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        string width = tg.Get(PdfName.WIDTH).ToString();
                        string height = tg.Get(PdfName.HEIGHT).ToString();
                        ImageRenderInfo imgRI = ImageRenderInfo.CreateForXObject(new Matrix(float.Parse(width), float.Parse(height)), (PRIndirectReference)obj, tg);

                        PdfImageObject pdfImageObject = imgRI.GetImage();

                        using (Image image = pdfImageObject.GetDrawingImage())
                        {
                            if (image.Height >= minimumHeight && image.Width >= minumumWidth)
                            {
                                if (pdfImageObject.GetDrawingImage() != null)
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        image.Save(ms, imageFormat);

                                        extractedImages.Add(ms.ToArray());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            pdfReader.Close();

            return extractedImages;
        }
    }
}