using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace akiss.GitHub.YetAnotherMimeMagic
{
    public static partial class MimeMagic
    {
        /// <summary>
        /// Initilaze collections
        /// </summary>
        private static void Initialize()
        {
            if (_allTypes == null || _allTypes.Count == 0)
            {
                // load mime info provided by http://www.freedesktop.org/standards/shared-mime-info"
                var serializer = new XmlSerializer(typeof(MimeInfo));
                var mimeTypesXml = Assembly.GetAssembly(typeof(MimeMagic)).GetManifestResourceStream("akiss.GitHub.YetAnotherMimeMagic.freedesktop.org.xml");
                var mimeinfo = (MimeInfo)serializer.Deserialize(mimeTypesXml);

                // drop all the currently unsupported items.
                _allTypes = mimeinfo.MimeTypes.Where(p => CanEvaluateSingleMimeType(p)).ToList();
            }
        }

        /// <summary>
        /// Select mimetype if it has at least one extension info or correct magic 
        /// </summary>
        private static bool CanEvaluateSingleMimeType(MimeType mimeType)
        {
            return
                (mimeType.Magic != null && CanEvaluateSingleMagic(mimeType.Magic)) ||
                mimeType.Globs.Count > 0;
        }

        /// <summary>
        /// Magic is correct if it is having at least one usable match info
        /// </summary>
        private static bool CanEvaluateSingleMagic(Magic magic)
        {
            var result = false;

            for (int i = magic.Matches.Count - 1; i >= 0; i--)
            {
                var item = magic.Matches[i];

                if (CanEvaluateSingleMatch(item))
                {
                    result = true;
                }
                else
                {
                    // delete all the unusable matches
                    magic.Matches.RemoveAt(i);
                }
            }

            return result;
        }

        /// <summary>
        /// Check a single match
        /// </summary>
        private static bool CanEvaluateSingleMatch(Match match)
        {
            // currently it is good enough for us, we can drop all the others
            var result = "string".Equals(match.Type) || "byte".Equals(match.Type);

            if (result)
            {
                try
                {
                    if ("string".Equals(match.Type))
                    {
                        match.Value = System.Text.RegularExpressions.Regex.Unescape(match.Value);
                    }

                    // check all the child matches if it is having any
                    if (match.Matches != null && match.Matches.Count > 0)
                    {
                        result = false;

                        for (int i = match.Matches.Count - 1; i >= 0; i--)
                        {
                            var item = match.Matches[i];

                            if (CanEvaluateSingleMatch(item))
                            {
                                result = true;
                            }
                            else
                            {
                                // delete all the unusable matches
                                match.Matches.RemoveAt(i);
                            }
                        }
                    }
                }
                // pretty lazy error handling :(
                catch
                {
                    result = false;
                }
            }

            return result;

        }
    }
}
