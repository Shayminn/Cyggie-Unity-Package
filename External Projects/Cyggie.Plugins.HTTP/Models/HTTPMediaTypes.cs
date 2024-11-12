namespace Cyggie.Plugins.HTTP.Models
{
    /// <summary>
    /// Const of HTTP media types
    /// </summary>
    public struct HTTPMediaTypes
    {
        /// <summary>
        /// Form submissions with data encoded as key-value pairs
        /// </summary>
        public const string cFormData = "application/x-www-form-urlencoded";

        /// <summary>
        /// Form submissions with file uploads
        /// </summary>
        public const string cMultipartFormData = "multipart/form-data";

        /// <summary>
        /// Data sent as JSON objects
        /// </summary>
        public const string cJSON = "application/json";

        /// <summary>
        /// Data sent as XML
        /// </summary>
        public const string cApplicationXML = "application/xml";

        /// <summary>
        /// XML in a text format
        /// </summary>
        public const string cTextXML = "text/xml";

        /// <summary>
        /// Plain text data
        /// </summary>
        public const string cText = "text/plain";

        /// <summary>
        /// Binary data
        /// </summary>
        public const string cBinary = "application/octet-stream";
    }
}
