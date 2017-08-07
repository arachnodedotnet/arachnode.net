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

#endregion

namespace Arachnode.Utilities
{
    public static class Directories
    {
        public static void DeleteDirectory(string directoryRoot)
        {
            try
            {
                if (!Directory.Exists(directoryRoot))
                {
                    return;
                }

                foreach (string directory in Directory.GetDirectories(directoryRoot))
                {
                    DeleteDirectory(directory);
                }

                string[] files2 = Directory.GetFiles(directoryRoot);

                if (files2.Length == 0)
                {
                    Directory.Delete(directoryRoot);

                    return;
                }
                foreach (string file in files2)
                {
                    File.Delete(file);
                }

                Directory.Delete(directoryRoot);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }
        }

        public static void DeleteFilesInDirectory(string directoryRoot)
        {
            if (!Directory.Exists(directoryRoot))
            {
                return;
            }

            foreach (string file in Directory.GetFiles(directoryRoot))
            {
                File.Delete(file);
            }

            foreach (string directory in Directory.GetDirectories(directoryRoot))
            {
                DeleteFilesInDirectory(directory);
            }

            foreach (string file in Directory.GetFiles(directoryRoot))
            {
                File.Delete(file);
            }
        }

        public static void FindFilesAndLinesInDirectory(string directoryRoot, string textToFind, bool isCaseSensitive, out List<string> foundFiles, out List<string> foundLines)
        {
            foundFiles = new List<string>();
            foundLines = new List<string>();

            if (!isCaseSensitive)
            {
                textToFind = textToFind.ToLowerInvariant();
            }

            FindFilesAndLinesInDirectory(directoryRoot, textToFind, isCaseSensitive, foundFiles, foundLines);
        }

        private static void FindFilesAndLinesInDirectory(string directoryRoot, string textToFind, bool isCaseSensitive, List<string> foundFiles, List<string> foundLines)
        {
            if (!Directory.Exists(directoryRoot))
            {
                return;
            }

            foreach (string file in Directory.GetFiles(directoryRoot))
            {
                string[] allLines = File.ReadAllLines(file);

                foreach (string line in allLines)
                {
                    if (isCaseSensitive)
                    {
                        if (line.Contains(textToFind))
                        {
                            if (!foundFiles.Contains(file))
                            {
                                foundFiles.Add(file);
                            }
                            foundLines.Add(line);
                        }
                    }
                    else
                    {
                        string line2 = line.ToLowerInvariant();

                        if (line2.Contains(textToFind))
                        {
                            if (!foundFiles.Contains(file))
                            {
                                foundFiles.Add(file);
                            }
                            foundLines.Add(line2);
                        }
                    }
                }
            }

            foreach (string directory in Directory.GetDirectories(directoryRoot))
            {
                FindFilesAndLinesInDirectory(directory, textToFind, isCaseSensitive, foundFiles, foundLines);
            }
        }
    }
}