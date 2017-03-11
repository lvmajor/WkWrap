using System;

namespace WkWrap.Core
{
    /// <summary>
    /// The exception that thrown when WkHtmlToPdf process retruns non-zero exit code.
    /// </summary>
    public class WkWrapException : Exception
    {
        /// <summary>
        /// Gets WkHtmlToPdf process error code.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Returns new instance of <see cref="WkWrapException"/>.
        /// </summary>
        /// <param name="errorCode">WkHtmlToPdf process error code.</param>
        /// <param name="message">WkHtmlToPdf error text.</param>
        public WkWrapException(int errorCode, string message) : base($"{message} ({errorCode:D})")
        {
            ErrorCode = errorCode;
        }
    }
}