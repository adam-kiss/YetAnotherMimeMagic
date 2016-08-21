using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace akiss.GitHub.YetAnotherMimeMagic
{
    /// <summary>
    /// Nice class to provide result
    /// </summary>
    public class MimeMagicResult
    {
        /// <summary>
        /// the result itself
        /// </summary>
        public string MimeType { get; internal set; }
        /// <summary>
        /// friendly name of result if available
        /// </summary>
        public string Acronym { get; internal set; }

        /// <summary>
        /// Was it found by the content?
        /// </summary>
        public bool FoundByContent { get; internal set; }
        /// <summary>
        /// Was it found by the filename extension?
        /// </summary>
        public bool FoundByExtension { get; internal set; }

        /// <summary>
        /// Execution time
        /// </summary>
        public int ElapsedtimeInMS { get; internal set; }
    }
}
