using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WkWrap.Core
{
    /// <summary>
    /// HtmlToPdfConverter public interface.
    /// </summary>
    public interface IHtmlToPdfConverter
    {
        /// <summary>
        /// Occurs when log line is received from WkHtmlToPdf process.
        /// </summary>
        /// <remarks>
        /// Quiet mode should be disabled if you want to get wkhtmltopdf info/debug messages.
        /// </remarks>
        event EventHandler<DataReceivedEventArgs> LogReceived;

        /// <summary>
        /// Generates PDF by specifed HTML content.
        /// </summary>
        /// <param name="html">HTML content.</param>
        /// <returns></returns>
        byte[] ConvertToPdf(string html);

        /// <summary>
        /// Generates PDF by specifed HTML content.
        /// </summary>
        /// <param name="html">HTML content.</param>
        /// <param name="htmlEncoding">HTML content encoding.</param>
        /// <param name="settings">Conversion settings.</param>
        /// <returns></returns>
        byte[] ConvertToPdf(string html, Encoding htmlEncoding, ConversionSettings settings);

        /// <summary>
        /// Generate PDF into specified output <see cref="Stream" />.
        /// </summary>
        /// <param name="inputStream">HTML content input stream.</param>
        /// <param name="outputStream">PDF file output stream.</param>
        /// <param name="settings">Conversion settings.</param>
        void ConvertToPdf(Stream inputStream, Stream outputStream, ConversionSettings settings);

        /// <summary>
        /// Generate PDF into specified output <see cref="Stream" />.
        /// </summary>
        /// <param name="inputStream">HTML content input stream.</param>
        /// <param name="outputStream">PDF file output stream.</param>
        /// <param name="settings">wkhtmltopdf command line arguments.</param>
        /// <param name="executionTimeout">Maximum execution time for PDF generation process (null means that no timeout).</param>
        void ConvertToPdf(Stream inputStream, Stream outputStream, string settings, TimeSpan? executionTimeout = null);
    }
}