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
using System.Text;
using Arachnode.Plugins.CrawlActions.Managers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Test
{
    ///<summary>
    ///	This is a test class for PDFManagerTest and is intended
    ///	to contain all PDFManagerTest Unit Tests
    ///</summary>
    [TestClass]
    public class PDFManagerTest
    {
        ///<summary>
        ///	Gets or sets the test context which provides
        ///	information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        ///<summary>
        ///	A test for ExtractText
        ///</summary>
        [TestMethod]
        public void ExtractTextTest1()
        {
            PDFManager pdfManager = new PDFManager(); // TODO: Initialize to an appropriate value
            //byte[] input = File.ReadAllBytes(DiscoveryManager.GetDiscoveryPath("M:\\DFD", "http://unicode.org/charts/PDF/U0590.pdf", ".pdf"));

            byte[] input = File.ReadAllBytes(@"");


            string path = @"M:\COL\hebrew.pdf";
            string destinationFileName = @"M:\COL\hebrew1.pdf";


            PdfReader reader = new PdfReader(path);
            int n = reader.NumberOfPages;
            Document document = new Document(PageSize.A4);

            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destinationFileName, FileMode.Create));

            int i = 0;
            document.Open();

            PdfContentByte cb = writer.DirectContent;


            PdfTemplate template = cb.CreateTemplate(0, 0);


            while (i < n)
            {
                document.NewPage();
                i++;

                PdfImportedPage importedPage = writer.GetImportedPage(reader, i);


                Image img = Image.GetInstance(importedPage);

                img.ScalePercent(100);
                document.Add(img);
                cb.AddTemplate(importedPage, 0, 100);
            }


            document.Close();
            writer.Close();


            PdfReader pdfReader = new PdfReader(input);

            StringBuilder stringBuilder = new StringBuilder();

            string dingle = string.Empty;

            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                stringBuilder.Append(pdfManager.ExtractText(pdfReader.GetPageContent(page)) + " ");

                PRTokeniser prTokeniser = new PRTokeniser(pdfReader.GetPageContent(page));


                PdfDictionary pdfDictionary = pdfReader.GetPageN(page);

                byte[] dinas = pdfReader.GetPageContent(page);

                string winsdgf = Encoding.GetEncoding(1255).GetString(dinas);


                try
                {
                    while (prTokeniser.NextToken())
                    {
                        if (prTokeniser.TokenType == PRTokeniser.TokType.STRING)
                        {
                            dingle += prTokeniser.StringValue;

                            try
                            {
                                //dingle += (char)(int.Parse(prTokeniser.StringValue));

                                //dingle += iTextSharp.text.Utilities.ConvertFromUtf32(prTokeniser.FilePointer);

                                //dingle += ((char)prTokeniser.Read()).ToString();

                                dingle += prTokeniser.ReadString(2);
                                Chunk chunk = new Chunk(prTokeniser.StringValue);

                                //string wangle = PRTokeniser.GetHex(prTokeniser.IntValue).ToString();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    {
                    }
                    //throw;
                }

                //int ij = 0;

//                #
//If Not IsNothing(pageBytes) Then
//#
//                    token = New PRTokeniser(pageBytes)
//#
//                    While token.NextToken()
//#
//                        tknType = token.TokenType()
//#
//                        tknValue = token.StringValue
//#
//                        If tknType = PRTokeniser.TK_STRING Then
//#
//                            sb.Append(token.StringValue)
//#
//                        'I need to add these additional tests to properly add whitespace to the output string
//#
//                        ElseIf tknType = 1 AndAlso tknValue = "-600" Then
//#
//                            sb.Append(" ")
//#
//                        ElseIf tknType = 10 AndAlso tknValue = "TJ" Then
//#
//                            sb.Append(" ")
//#
//                        End If
//#
//                   End While 
            }

            string actual = pdfManager.ExtractText(input);
        }

        ///<summary>
        ///	A test for ExtractText
        ///</summary>
        [TestMethod]
        public void ExtractTextTest()
        {
            PDFManager target = new PDFManager(); // TODO: Initialize to an appropriate value
            string inFileName = string.Empty; // TODO: Initialize to an appropriate value
            string outFileName = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ExtractText(inFileName, outFileName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}