using akiss.GitHub.YetAnotherMimeMagic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akiss.GitHub.YammConsole
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Console.WriteLine("Usage: \n YammConsole.exe [file|directory]");
                return;
            }

            bool isDirectory = Directory.Exists(args[0]);
            bool isFile = File.Exists(args[0]);

            if (!isDirectory && !isFile)
            {
                Console.WriteLine("File or directory not found.");
                return;
            }

            var files = new List<FileInfo>();

            if (isFile)
            {
                files.Add(new FileInfo(args[0]));
            }

            if (isDirectory)
            {
                var entries = Directory.EnumerateFiles(args[0], "*.*", SearchOption.AllDirectories);

                foreach (var entry in entries)
                {
                    files.Add(new FileInfo(entry));
                }
            }

            var results = new List<MimeMagicResult>();

            foreach (var file in files)
            {
                try
                {
                    results.Add(file.FindMimeType());
                }
                catch
                {
                    results.Add(null);
                }
            }

            if (files.Count == 1)
            {
                Console.WriteLine($" filename:           { files[0].FullName }");
                Console.WriteLine($" mime-type:          { results[0]?.MimeType }");
                Console.WriteLine($" acronym:            { results[0]?.Acronym }");
                Console.WriteLine($" found by content:   { results[0]?.FoundByContent }");
                Console.WriteLine($" found by extension: { results[0]?.FoundByExtension }");
                Console.WriteLine($" elasped time in ms: { results[0]?.ElapsedtimeInMS }");
            }
            else if (files.Count > 1)
            {
                var resultname = "result" + DateTime.Now.Ticks + ".csv";
                var stream = File.OpenWrite(resultname);

                var writer = new StreamWriter(stream);

                writer.WriteLine(" filename; mime-type; acronym; found by content; found by extension; elasped time in ms");

                for (int i = 0; i < files.Count; i++)
                {
                    writer.WriteLine($" { files[i].FullName };  { results[i]?.MimeType }; { results[i]?.Acronym }; { results[i]?.FoundByContent }; { results[i]?.FoundByExtension }; { results[i]?.ElapsedtimeInMS }");
                }

                writer.Flush();
                writer.Close();

                Console.Write("Result was saved to " + resultname);
            }

        }
    }

}
