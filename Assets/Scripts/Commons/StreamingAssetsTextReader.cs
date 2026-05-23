using System.IO;
using UnityEngine;

namespace Commons
{
    public static class StreamingAssetsTextReader
    {
        public static string GetTextByName(string fileName)
        {
            var filePaths = GetFilePaths($"*{fileName}");

            int length = filePaths.Length;
            for (int i = 0; i < length; i++)
            {
                string path = filePaths[i];

                if (Path.GetFileName(path) == fileName)
                    return File.ReadAllText(path);
            }

            return $"File {fileName} not found.";
        }

        public static string GetTextByPath(string path)
        {
            if(File.Exists(path) is false)
                return $"File {path} not found.";

            return File.ReadAllText(path);
        }

        public static string[] GetFileNames()
        {
            var filePaths = GetFilePaths();

            int length = filePaths.Length;
            string[] files = new string[length];

            for (int i = 0; i < length; i++)
            {
                string file = Path.GetFileName(filePaths[i]);
                files[i] = file;
            }

            return files;
        }

        public static string[] GetFilePaths(string searchPattern = "*.txt")
        {
            return Directory.GetFiles(Application.streamingAssetsPath, searchPattern,
                SearchOption.AllDirectories);
        }
    }
}
