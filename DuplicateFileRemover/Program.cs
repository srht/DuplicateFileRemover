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
            var files = Directory.GetFiles(args[0]).Select(i => new FileInfo(i)).ToList();
            var matchedFiles = new List<FileInfo>();

            FileInfo targetFileInfo = files.First();

            string pattern = @"[\w\x20][(][0-9]+[)]";
            
            foreach (var f in files)
            {
                targetFileInfo = f;

                string targetFullNameBeginVal =
                    targetFileInfo.FullName.Remove(
                        Regex.IsMatch(targetFileInfo.FullName, pattern)?
                        Regex.Match(targetFileInfo.FullName, pattern).Index
                        :targetFileInfo.FullName.IndexOf(targetFileInfo.Extension));
                
                var filteredFiles = files.Where(m => m.Length == targetFileInfo.Length
                                                && m.FullName.StartsWith(targetFullNameBeginVal)
                                                && Regex.IsMatch(m.FullName, pattern)
                                                && targetFileInfo.Exists
                                                && m.Extension == targetFileInfo.Extension);

                if (filteredFiles.Count() > 0 && !matchedFiles.Any(i=>i.FullName.StartsWith(targetFullNameBeginVal)))
                {
                    matchedFiles.AddRange(filteredFiles);
                }

            }

            long totalDeletedSize = 0;

            foreach (var mf in matchedFiles)
            {
                Console.WriteLine($"{mf},");
            }

            Console.WriteLine("These files will be deleted permanently OK? Type OK if you are OK.");
            string answer = Console.ReadLine();

            if (answer=="OK")
            {
                foreach (var mf in matchedFiles)
                {
                    File.Delete(mf.FullName);
                    totalDeletedSize += mf.Length;
                    Console.WriteLine($"{mf},");
                }

                Console.WriteLine($"Files deleted. {(totalDeletedSize / 1000000)} megabyte size saved on disk.");

                Console.Read();
            }
            
        }
    }
}
