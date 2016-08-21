using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akiss.GitHub.YetAnotherMimeMagic
{
    /// <summary>
    /// SharpMimeMagic is a library to detect the mime type of a file by content with a fallback way by its extension. 
    /// It uses the mime database provided by freedesktop.org (see http://freedesktop.org/wiki/Software/shared-mime-info/)
    /// </summary>
    public static partial class MimeMagic
    {
        private static readonly int _bufferLength = 4096;
        private static List<MimeType> _allTypes;

        /// <summary>
        /// Detect the mime type of file based by content with a fallback way by its extension
        /// </summary>
        /// <returns>instance of MimeMagicResult</returns>
        public static MimeMagicResult FindMimeType(this FileInfo info)
        {
            if (info.Exists)
            {
                using (var stream = info.OpenRead())
                {
                    return FindMimeType(stream as Stream, info.FullName);
                }
            }

            return null;
        }

        /// <summary>
        /// Detect the mime type of file based by content with a fallback way by its extension
        /// </summary>
        /// <returns>instance of MimeMagicResult</returns>
        public static MimeMagicResult FindMimeType(this MemoryStream contentStream, string filename = "")
        {
            return FindMimeType(contentStream as Stream, filename);
        }

        /// <summary>
        /// Detect the mime type of file based by content with a fallback way by its extension
        /// </summary>
        /// <returns>instance of MimeMagicResult</returns>
        public static MimeMagicResult FindMimeType(this FileStream contentStream, string filename = "")
        {
            return FindMimeType(contentStream as Stream, filename);
        }

        /// <summary>
        /// For seekable streams
        /// </summary>
        private static MimeMagicResult FindMimeType(Stream contentStream, string filename = "")
        {
            var length = (int)Math.Min(_bufferLength, contentStream.Length);
            var contentBytes = new byte[length];

            contentStream.Seek(0, SeekOrigin.Begin);
            contentStream.Read(contentBytes, 0, length);

            return FindMimeType(contentBytes, filename);
        }

        /// <summary>
        /// Detect the mime type of file based by content with a fallback way by its extension
        /// </summary>
        /// <returns>instance of MimeMagicResult</returns>
        public static MimeMagicResult FindMimeType(this byte[] contentBytes, string filename = "")
        {
            var result = new MimeMagicResult() { MimeType = "application/octet-stream" };
            var watch = new Stopwatch();
            watch.Start();

            Initialize();

            try
            {
                var contentString = Encoding.UTF7.GetString(contentBytes, 0, Math.Min(_bufferLength, contentBytes.Length));

                var extension = "*" + Path.GetExtension(filename);

                // first, check all the base types
                var mimes = _allTypes;
                do
                {
                    var found = mimes.Find(mime =>
                    {
                        return EvaluateSingleMime(extension, contentString, mime, result);
                    });

                    // is there any sub type for this type
                    mimes = _allTypes.Where(p => p.SubClassOf != null && p.SubClassOf.Type == found?.Type).ToList();

                    // set the result
                    if (found != null)
                    {
                        result.Acronym = string.IsNullOrEmpty(found?.ExpandedAcronym) ? found?.Comment : found.ExpandedAcronym;
                        result.MimeType = found?.Type;
                    }
                }
                while (mimes != null && mimes.Count > 0);

                return result;
            }
            finally
            {
                watch.Stop();

                result.ElapsedtimeInMS = (int)watch.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// According to its name
        /// </summary>
        private static bool EvaluateSingleMime(string extension, string content, MimeType mime, MimeMagicResult diagnostic)
        {
            var matching = false;

            // try to find the magic in the content
            if (mime.Magic != null && mime.Magic.Matches != null && mime.Magic.Matches.Count > 0)
            {
                matching = mime.Magic.Matches.Any(child => EvaluateSingleMatchRecursively(content, child));

                if (matching)
                    diagnostic.FoundByContent = true;
            }

            // fallback to the file extension.
            if (!matching)
            {
                matching = mime.Globs.Any(ext => extension.Equals(ext.Pattern));

                if (matching)
                    diagnostic.FoundByExtension = true;
            }

            return matching;
        }

        /// <summary>
        /// According to its name
        /// </summary>
        private static bool EvaluateSingleMatchRecursively(string content, Match match)
        {
            var result = false;
            int start = 0;
            int end = 0;

            // try for exact match
            if (int.TryParse(match.Offset, out start))
            {
                // for utf-8 encoded text files with bom
                var croped = new string(content.Skip(start).ToArray().Take(Math.Min(match.SearchPattern.Length + 4, content.Length - start)).ToArray());
                result = croped.Contains(match.SearchPattern);
            }
            else
            {
                // try for range
                var bounds = match.Offset.Split(new string[] { ":" }, StringSplitOptions.None);
                if (bounds.Length == 2 &&
                    int.TryParse(bounds[0], out start) &&
                    int.TryParse(bounds[1], out end))
                {
                    var croped = new string(content.Skip(start).ToArray().Take(Math.Min(end - start, content.Length - start)).ToArray());
                    result = croped.Contains(match.SearchPattern);
                }
            }

            // check the childs 
            if (result && match.Matches != null && match.Matches.Count > 0)
            {
                result = match.Matches.Any(child => EvaluateSingleMatchRecursively(content, child));
            }

            return result;
        }
    }

}
