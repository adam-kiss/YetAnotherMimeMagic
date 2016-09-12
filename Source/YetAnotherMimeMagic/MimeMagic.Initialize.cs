using System;
using System.Collections.Generic;
using System.Globalization;
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

                // drop all the currently unsupported items and priorize types with magic
                _allTypes = mimeinfo.MimeTypes.Where(p => CanEvaluateSingleMimeType(p)).OrderBy(p => p.Magic == null).ThenBy(p=>p.SubClassOf != null).ToList();
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
            var result = false;

            try
            {
                // check for known types and preprocess 
                switch (match.Type)
                {
                    case "string":
                        match.SearchPattern = System.Text.RegularExpressions.Regex.Unescape(match.Value);
                        result = true;
                        break;

                    case "big32":
                        var bbytes32 = GetbytesFromStringInteger(match.Value);
                        if (bbytes32 != null)
                        {
                            match.SearchPattern = new string(new char[] { (char)bbytes32[3], (char)bbytes32[2], (char)bbytes32[1], (char)bbytes32[0] });
                        }
                        result = true;
                        break;

                    case "big16":
                        var bbytes16 = GetbytesFromStringInteger(match.Value);
                        if (bbytes16 != null)
                        {
                            match.SearchPattern = new string(new char[] { (char)bbytes16[1], (char)bbytes16[0] });
                        }
                        result = true;
                        break;

                    case "little32":
                    case "host32":
                        var lbytes32 = GetbytesFromStringInteger(match.Value);
                        if (lbytes32 != null)
                        {
                            match.SearchPattern = new string(new char[] { (char)lbytes32[0], (char)lbytes32[1], (char)lbytes32[2], (char)lbytes32[3] });
                        }
                        result = true;
                        break;

                    case "little16":
                    case "host16":
                        var lbytes16 = GetbytesFromStringInteger(match.Value);
                        if (lbytes16 != null)
                        {
                            match.SearchPattern = new string(new char[] { (char)lbytes16[0], (char)lbytes16[1] });
                        }
                        result = true;
                        break;

                    case "byte":
                        if (match.Value.StartsWith("0x"))
                        {
                            match.SearchPattern = ((char)int.Parse(match.Value.Substring(2), NumberStyles.HexNumber)).ToString();
                        }
                        else
                        {
                            match.SearchPattern = ((char)int.Parse(match.Value)).ToString();
                        }
                        result = true;
                        break;

                    default:
                        result = false;
                        break;
                }

                if (result)
                {
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
            }
            // pretty lazy error handling :(
            catch (Exception ex)
            {
                result = false;
            }

            return result;

        }

        private static byte[] GetbytesFromStringInteger(string stringValue)
        {
            uint value;
            if (stringValue.StartsWith("0x"))
            {
                if (uint.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out value))
                {
                    return BitConverter.GetBytes(value);
                }
            }
            else
            {
                if (uint.TryParse(stringValue, out value))
                {
                    return BitConverter.GetBytes(value);
                }
            }

            return null;
        }
    }
}
