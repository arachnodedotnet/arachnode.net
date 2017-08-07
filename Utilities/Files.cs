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
using System.IO;
using System.Text;

#endregion

namespace Arachnode.Utilities
{
    public class Files
    {
        /// <summary>
        /// 	Extracts Files and Images from the database, saves to disk and updates File and Image MetaData according to ApplicationSettings.
        /// 	This method is provided for retroactively performing tasks that may have been disabled during crawling.
        /// </summary>
        /// <param name = "fromFileID"></param>
        /// <param name = "toFileID"></param>
        private static void ExtractFilesAndInsertFilesMetaData(long fromFileID, long toFileID)
        {
            //for (long i = fromFileID; i <= toFileID; i++)
            //{
            //    ArachnodeDataSet.FilesRow filesRow = _arachnodeDAO.GetFile(i.ToString());

            //    if (filesRow != null)
            //    {
            //        switch (_dataTypeManager.DetermineDataType(filesRow.AbsoluteUri, filesRow.ResponseHeaders).ContentType)
            //        {
            //            case ContentType.File:
            //                _fileManager.ManageFile(null, filesRow.ID, filesRow.AbsoluteUri, filesRow.Source, filesRow.FullTextIndexType, true, true, false);

            //                System.Console.WriteLine("File Extracted. " + i);
            //                break;
            //            case ContentType.Image:
            //                _fileManager.ManageImage(null, filesRow.ID, filesRow.AbsoluteUri, filesRow.Source, filesRow.FullTextIndexType, true, true, false);

            //                System.Console.WriteLine("Image Extracted. " + i);
            //                break;
            //        }
            //    }

            //    System.Console.WriteLine(i.ToString());
            //}
        }

        public static void PrepareSiteCrawlerProjectSourceFilesForDemo()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("..\\..\\..\\SiteCrawler");

            PrepareSiteCrawlerProjectSourceFilesForDemo(directoryInfo, "//Full source code for the 'SiteCrawler' project is included in the 'Licensed' version.  (C#/T-SQL/Database)");
        }

        private static void PrepareSiteCrawlerProjectSourceFilesForDemo(DirectoryInfo directoryInfo, string fileContentsReplacementString)
        {
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (fileInfo.Name.EndsWith(".cs"))
                {
                    //Replace the file contents with the content replacement string...
                    Console.WriteLine("Preparing " + fileInfo.Name + " for DEMO.");

                    Random random = new Random(Environment.TickCount);

                    StringBuilder stringBuilder = new StringBuilder();

                    string[] allLines = File.ReadAllLines(fileInfo.FullName);

                    for (int i = 0; i < allLines.Length; i++)
                    {
                        string preparedLine = allLines[i];

                        for (int j = 0; j < 50; j++)
                        {
                            preparedLine = preparedLine.Replace((char) random.Next('!', 'z'), (char) random.Next('!', 'z'));
                        }

                        stringBuilder.AppendLine("//" + preparedLine);
                    }

                    File.WriteAllText(fileInfo.FullName, fileContentsReplacementString + Environment.NewLine + stringBuilder, Encoding.Default);
                }
            }

            foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
            {
                PrepareSiteCrawlerProjectSourceFilesForDemo(directoryInfo2, fileContentsReplacementString);
            }
        }

        public static List<Uri> ExtractAbsoluteUris(string path)
        {
            List<Uri> uris = new List<Uri>();

            try
            {
                if (File.Exists(path))
                {
                    foreach (string line in File.ReadAllLines(path))
                    {
                        Uri uri = null;

                        if (Uri.TryCreate(line, UriKind.Absolute, out uri))
                        {
                            uris.Add(uri);
                        }
                        else
                        {
                            if (Uri.TryCreate("http://" + line, UriKind.Absolute, out uri))
                            {
                                uris.Add(uri);
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return uris;
        }
    }
}