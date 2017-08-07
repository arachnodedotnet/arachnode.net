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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Arachnode.Configuration;
using Arachnode.DataAccess;
using Arachnode.DataSource;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Test
{
    /// <summary>
    /// 	Summary description for ManageLuceneDotNetIndexesTest
    /// </summary>
    [TestClass]
    public class ManageLuceneDotNetIndexesTest
    {
#pragma warning disable 612,618
        ///<summary>
        ///	Gets or sets the test context which provides
        ///	information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [TestMethod]
        public void TestThatAllWebPagesAreInTheIndex()
        {
            int minID;
            int maxID;

            SqlCommand sqlCommand = new SqlCommand("Select Min(ID) From WebPages");

            sqlCommand.Connection = new SqlConnection("Data Source=.;Initial Catalog=arachnode.net;Integrated Security=True;Connection Timeout=3600;");
            sqlCommand.Connection.Open();

            using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
            {
                sqlDataReader.Read();
                minID = int.Parse(sqlDataReader.GetValue(0).ToString());
            }

            sqlCommand.CommandText = "Select Max(ID) From WebPages";

            using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
            {
                sqlDataReader.Read();
                maxID = int.Parse(sqlDataReader.GetValue(0).ToString());
            }

            ApplicationSettings applicationSettings = new ApplicationSettings();

            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(applicationSettings.ConnectionString);

            IndexSearcher _indexSearcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo("M:\\LDNI")), true);
            StandardAnalyzer standardAnalyzer = new StandardAnalyzer();
            QueryParser queryParser = new QueryParser("discoveryid", standardAnalyzer);

            for (int i = minID; i <= maxID; i++)
            {
                Debug.Print(i.ToString());

                ArachnodeDataSet.WebPagesRow webPagesRow = arachnodeDAO.GetWebPage(i.ToString());

                Query query = queryParser.Parse("\"" + webPagesRow.ID + "\"");

                Hits hits = _indexSearcher.Search(query);

                bool constainsTheWebPageAbsoluteUri = false;

                for (int j = 0; j < hits.Length(); j++)
                {
                    if (hits.Doc(j).GetField("discoverytype").StringValue() == "webpage")
                    {
                        constainsTheWebPageAbsoluteUri = true;
                    }
                }

                if (!constainsTheWebPageAbsoluteUri)
                {
                    //ANODET: Set Breakpoint...
                }

                Assert.IsTrue(constainsTheWebPageAbsoluteUri);
            }

            sqlCommand.Connection.Close();
        }

        [TestMethod]
        public void TestForDuplicatesInTheIndex()
        {
            ApplicationSettings applicationSettings = new ApplicationSettings();

            ArachnodeDAO arachnodeDAO = new ArachnodeDAO(applicationSettings.ConnectionString);

            IndexSearcher _indexSearcher = new IndexSearcher(FSDirectory.Open(new DirectoryInfo("M:\\LDNI")), true);
            StandardAnalyzer standardAnalyzer = new StandardAnalyzer();
            QueryParser queryParser = new QueryParser("discoveryid", standardAnalyzer);

            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            for (int i = 0; i < _indexSearcher.GetIndexReader().NumDocs(); i++)
            {
                Debug.Print(i.ToString());

                Document document = _indexSearcher.Doc(i);

                string absoluteUri = document.GetField("absoluteuri").StringValue();

                if (!dictionary.ContainsKey(absoluteUri))
                {
                    try
                    {
                        dictionary.Add(absoluteUri, new List<string>());
                        dictionary[absoluteUri].Add(document.GetField("discoveryid").ToString());
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        dictionary[absoluteUri].Add(document.GetField("discoveryid").ToString());
                        //Assert.Fail("Each AbsoluteUri should be present only once in the Lucene.net index.");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            object someting = dictionary.Where(d => (d.Value).Count > 1).ToArray();
        }
    }
}