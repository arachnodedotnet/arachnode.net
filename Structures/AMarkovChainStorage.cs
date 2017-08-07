using System;
using System.Diagnostics;
using System.IO;

namespace Arachnode.Structures
{
    public class AMarkovChainStorage
    {
        internal string _onDiskDirectoryBasePath { get; set; }
        protected const string _onDiskDirectoryFileName = "MarkovChainNodeString.txt";

        public const string ROOT = "ROOT";

        private string _path;
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(_path))
                {
                    _path = ROOT;
                }

                return _path;
            }
            set { _path = value; }
        }

        protected string GetActualOnDiskDirectoryPath()
        {
            string directoryPath = _onDiskDirectoryBasePath;

            if (!string.IsNullOrEmpty(Path) && Path != ROOT)
            {
                directoryPath = System.IO.Path.Combine(_onDiskDirectoryBasePath, Path);
            }

            return directoryPath;
        }

        protected string GetActualOnDiskFilePath(string childKey)
        {
            string directoryPath = GetActualOnDiskDirectoryPath();

            if(!string.IsNullOrEmpty(childKey))
            {
                directoryPath = System.IO.Path.Combine(directoryPath, childKey + "\\");
            }

            string filePath = System.IO.Path.Combine(directoryPath, _onDiskDirectoryFileName);

            return filePath;
        }

        protected string GetKeyPath(string directoryPath)
        {
            string[] directoryPathSplit = directoryPath.Split("\\".ToCharArray());

            directoryPath = directoryPathSplit[directoryPathSplit.Length - 1];

            return directoryPath;
        }
    }
}