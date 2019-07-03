using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace DuplicateFileRemover
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(args[0]).Select(i => new FileInfo(i));
            var matchedFiles = new List<string>();

            FileInfo targetFileInfo = files.First();

            string pattern = @"[\w\x20][(][0-9]+[)]";
            
            foreach (var f in files)
            {
                targetFileInfo = f;

                string targetFullNameBeginVal = targetFileInfo.FullName
                    .Remove(Regex.Match(targetFileInfo.FullName, pattern).Index);

                var filteredFiles = files.Where(m => m.Length == targetFileInfo.Length
                                                && m.FullName.StartsWith(targetFullNameBeginVal)
                                                && Regex.IsMatch(m.FullName, pattern)
                                                && targetFileInfo.Exists
                                                && m.Extension == targetFileInfo.Extension);

                if (filteredFiles.Count() > 1 && !matchedFiles.Any(i=>i.StartsWith(targetFullNameBeginVal)))
                {
                    matchedFiles.AddRange(filteredFiles.Select(i=>i.FullName));
                }

            }

            for (int i = 0; i < matchedFiles.Count; i++)
            {
                File.Delete(matchedFiles[i]);
                Console.WriteLine(matchedFiles[i]+",");
            }

            Console.WriteLine("Dosyaları silindi.");

            Console.Read();
        }
    }
}
