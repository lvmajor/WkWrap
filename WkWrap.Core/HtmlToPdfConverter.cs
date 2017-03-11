using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WkWrap.Core
{
    /// <summary>
    /// Html to PDF converter (C# WkHtmlToPdf process wrapper).
    /// </summary>
    public class HtmlToPdfConverter : IHtmlToPdfConverter
    {
        /// <summary>
        /// Constructs new instance of <see cref="HtmlToPdfConverter"/>.
        /// </summary>
        /// <param name="wkHtmlToPdfExecutableFile">wkhtmltopdf executable file.</param>
        public HtmlToPdfConverter(
            FileInfo wkHtmlToPdfExecutableFile)
        {
            if (wkHtmlToPdfExecutableFile == null)
                throw new ArgumentNullException(nameof(wkHtmlToPdfExecutableFile));
            if (wkHtmlToPdfExecutableFile.Exists != true)
                throw new FileNotFoundException("wkhtmltopdf executable file not found!");
            WkHtmlToPdfExecutableFile = wkHtmlToPdfExecutableFile;
        }

        /// <summary>
        /// Returns wkhtmltopdf executable file.
        /// </summary>
        private FileInfo WkHtmlToPdfExecutableFile { get; }

        /// <summary>
        /// Instance of wkhtmltopdf working process.
        /// </summary>
        private Process _wkHtmlToPdfProcess;

        /// <summary>
        /// Occurs when log line is received from WkHtmlToPdf process.
        /// </summary>
        /// <remarks>
        /// Quiet mode should be disabled if you want to get wkhtmltopdf info/debug messages.
        /// </remarks>
        public event EventHandler<DataReceivedEventArgs> LogReceived;

        /// <summary>
        /// Generates PDF by specifed HTML content.
        /// </summary>
        /// <param name="html">HTML content.</param>
        /// <returns></returns>
        public byte[] ConvertToPdf(string html)
        {
            return ConvertToPdf(html, Encoding.UTF8, ConversionSettings.Default());
        }

        /// <summary>
        /// Generates PDF by specifed HTML content.
        /// </summary>
        /// <param name="html">HTML content.</param>
        /// <param name="htmlEncoding">HTML content encoding.</param>
        /// <param name="settings">Conversion settings.</param>
        /// <returns></returns>
        public byte[] ConvertToPdf(string html, Encoding htmlEncoding, ConversionSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(html))
                html = string.Empty;
            byte[] result;
            using (var inputStream = new MemoryStream(htmlEncoding.GetBytes(html)))
            using (var outputStream = new MemoryStream())
            {
                ConvertToPdf(inputStream, outputStream, settings);
                result = outputStream.ToArray();
            }
            return result;
        }

        /// <summary>
        /// Generate PDF into specified output <see cref="Stream" />.
        /// </summary>
        /// <param name="inputStream">HTML content input stream.</param>
        /// <param name="outputStream">PDF file output stream.</param>
        /// <param name="settings">Conversion settings.</param>
        public void ConvertToPdf(
            Stream inputStream,
            Stream outputStream,
            ConversionSettings settings)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            ConvertToPdfInternal(inputStream, outputStream, settings.ToString().Trim(), settings.ExecutionTimeout);
        }

        /// <summary>
        /// Generate PDF into specified output <see cref="Stream" />.
        /// </summary>
        /// <param name="inputStream">HTML content input stream.</param>
        /// <param name="outputStream">PDF file output stream.</param>
        /// <param name="settings">wkhtmltopdf command line arguments.</param>
        /// <param name="executionTimeout">Maximum execution time for PDF generation process (null means that no timeout).</param>
        public void ConvertToPdf(
            Stream inputStream,
            Stream outputStream,
            string settings,
            TimeSpan? executionTimeout = null)
        {
            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));

            var stringSetting = settings?.Trim() ?? string.Empty;
            ConvertToPdfInternal(inputStream, outputStream, stringSetting, executionTimeout);
        }

        /// <summary>
        /// Generate PDF into specified output <see cref="Stream" />.
        /// </summary>
        /// <param name="inputStream">HTML content input stream.</param>
        /// <param name="outputStream">PDF file output stream.</param>
        /// <param name="settings">wkhtmltopdf command line arguments.</param>
        /// <param name="executionTimeout">Maximum execution time for PDF generation process (null means that no timeout).</param>
        private void ConvertToPdfInternal(
            Stream inputStream,
            Stream outputStream,
            string settings,
            TimeSpan? executionTimeout)
        {
            try
            {
                CheckWkHtmlProcess();
                InvokeWkHtmlToPdf(inputStream, outputStream, settings, executionTimeout);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot generate PDF: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Invokes wkhtmltopdf programm.
        /// </summary>
        /// <param name="inputStream">HTML content input stream.</param>
        /// <param name="outputStream">PDF file output stream.</param>
        /// <param name="settings">Conversion settings.</param>
        /// <param name="executionTimeout">Maximum execution time for PDF generation process (null means that no timeout).</param>
        private void InvokeWkHtmlToPdf(
            Stream inputStream,
            Stream outputStream,
            string settings,
            TimeSpan? executionTimeout)
        {
            var arguments = $"{settings} - -";
            try
            {
                _wkHtmlToPdfProcess =
                    Process.Start(new ProcessStartInfo(WkHtmlToPdfExecutableFile.FullName, arguments)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = WkHtmlToPdfExecutableFile.Directory.FullName,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    });
                _wkHtmlToPdfProcess.ErrorDataReceived += ErrorDataHandler;
                _wkHtmlToPdfProcess.BeginErrorReadLine();
                inputStream.CopyTo(_wkHtmlToPdfProcess.StandardInput.BaseStream);
                _wkHtmlToPdfProcess.StandardInput.BaseStream.Flush();
                _wkHtmlToPdfProcess.StandardInput.Dispose();
                _wkHtmlToPdfProcess.StandardOutput.BaseStream.CopyTo(outputStream);
                WaitWkHtmlProcessForExit(executionTimeout);
                CheckExitCode(_wkHtmlToPdfProcess.ExitCode, _lastLogLine);
            }
            finally
            {
                EnsureWkHtmlProcessStopped();
                _lastLogLine = null;
            }
        }

        /// <summary>
        /// Stores last log line.
        /// </summary>
        private string _lastLogLine;

        /// <summary>
        /// Error handler to rethrow wkhtmltopdf log events.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Instance of <see cref="DataReceivedEventArgs"/>.</param>
        private void ErrorDataHandler(
            object sender,
            DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e?.Data))
            {
                _lastLogLine = e.Data;
                LogReceived?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Check that wkhtmltopdf process is not running.
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws when wkhtmltopdf process is runnning.</exception>
        private void CheckWkHtmlProcess()
        {
            if (_wkHtmlToPdfProcess != null)
                throw new InvalidOperationException("WkHtmlToPdf process is already started");
        }

        /// <summary>
        /// Waits for wkhtmltopdf process ending.
        /// </summary>
        /// <param name="executionTimeout">wkhtmltopdf execution timeout.</param>
        private void WaitWkHtmlProcessForExit(TimeSpan? executionTimeout)
        {
            if (executionTimeout.HasValue)
            {
                if (!_wkHtmlToPdfProcess.WaitForExit((int) executionTimeout.Value.TotalMilliseconds))
                {
                    EnsureWkHtmlProcessStopped();
                    throw new WkWrapException(
                        -2,
                        string.Format(
                            "WkHtmlToPdf process exceeded execution timeout ({0}) and was aborted",
                            executionTimeout));
                }
            }
            else
                _wkHtmlToPdfProcess.WaitForExit();
        }

        /// <summary>
        /// Stops wkhtmltopdf process if it is not stopped.
        /// </summary>
        private void EnsureWkHtmlProcessStopped()
        {
            if (_wkHtmlToPdfProcess == null)
                return;
            if (!_wkHtmlToPdfProcess.HasExited)
            {
                try
                {
                    _wkHtmlToPdfProcess.Kill();
                    _wkHtmlToPdfProcess = null;
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            else
                _wkHtmlToPdfProcess = null;
        }

        /// <summary>
        /// Checks wkhtmltopdf's exit code.
        /// </summary>
        /// <param name="exitCode">Exit code.</param>
        /// <param name="lastErrorLine">Last error line.</param>
        private void CheckExitCode(int exitCode, string lastErrorLine)
        {
            if (exitCode != 0)
            {
                if (exitCode == 1 && Array.IndexOf(IgnoredWkHtmlToPdfErrors, lastErrorLine) > -1)
                {
                    return;
                }
                throw new WkWrapException(exitCode, lastErrorLine);
            }
        }

        /// <summary>
        /// Ignored WkHtmlToPdf errors.
        /// </summary>
        private static readonly string[] IgnoredWkHtmlToPdfErrors =
        {
            "Exit with code 1 due to network error: HostNotFoundError",
            "Exit with code 1 due to network error: ContentNotFoundError",
            "Exit with code 1 due to network error: ContentOperationNotPermittedError",
            "Exit with code 1 due to network error: ProtocolUnknownError",
            "Exit with code 1 due to network error: UnknownContentError",
            "QFont::setPixelSize: Pixel size <= 0"
        };
    }
}