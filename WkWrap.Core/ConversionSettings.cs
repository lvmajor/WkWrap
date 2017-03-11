using System;
using System.Globalization;
using System.Text;

namespace WkWrap.Core
{
    /// <summary>
    /// Represents settings, that using for HTML to PDF conversion.
    /// </summary>
    public class ConversionSettings
    {
        /// <summary>
        /// Constructs new instance of <see cref="ConversionSettings"/>.
        /// </summary>
        public ConversionSettings() : this(
            PageSize.Default,
            PageOrientation.Default,
            PageMargins.Default(),
            false,
            false,
            true,
            true,
            TimeSpan.FromMilliseconds(200),
            true,
            true,
            null)
        {
        }

        /// <summary>
        /// Constructs new instance of <see cref="ConversionSettings"/>.
        /// </summary>
        /// <param name="pageSize">PDF page size.</param>
        /// <param name="orientation">PDF page orientation.</param>
        /// <param name="margins">PDF page margins.</param>
        /// <param name="grayscale">Option to generate grayscale PDF.</param>
        /// <param name="lowQuality">Option to generate low quality PDF.</param>
        /// <param name="quiet">Option to suppress wkhtmltopdf debug/info log messages.</param>
        /// <param name="enableJavaScript">Option that allows web pages to run JavaScript.</param>
        /// <param name="javaScriptDelay">Delay for JavaScript finish (will applies only if JavaScript enabled).</param>
        /// <param name="enableExternalLinks">Option that allows make links to remote web pages.</param>
        /// <param name="enableImages">Option that allows to load or print images.</param>
        /// <param name="executionTimeout">Maximum execution time for PDF generation process.</param>
        public ConversionSettings(
            PageSize pageSize,
            PageOrientation orientation,
            PageMargins margins,
            bool grayscale,
            bool lowQuality,
            bool quiet,
            bool enableJavaScript,
            TimeSpan javaScriptDelay,
            bool enableExternalLinks,
            bool enableImages,
            TimeSpan? executionTimeout)
        {
            if (!Enum.IsDefined(typeof(PageSize), pageSize))
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (!Enum.IsDefined(typeof(PageOrientation), orientation))
                throw new ArgumentOutOfRangeException(nameof(orientation));
            if (javaScriptDelay.Ticks <= 0)
                throw new ArgumentOutOfRangeException(nameof(javaScriptDelay));
            PageSize = pageSize;
            Orientation = orientation;
            Margins = margins;
            Grayscale = grayscale;
            LowQuality = lowQuality;
            Quiet = quiet;
            EnableJavaScript = enableJavaScript;
            JavaScriptDelay = javaScriptDelay;
            EnableExternalLinks = enableExternalLinks;
            EnableImages = enableImages;
            if (executionTimeout.HasValue)
                ExecutionTimeout = executionTimeout.Value;
        }

        /// <summary>
        /// Returns PDF page size.
        /// </summary>
        private PageSize PageSize { get; }

        /// <summary>
        /// Returns PDF page orientation.
        /// </summary>
        private PageOrientation Orientation { get; }

        /// <summary>
        /// Returns PDF page margins.
        /// </summary>
        private PageMargins Margins { get; }

        /// <summary>
        /// Returns option to generate grayscale PDF.
        /// </summary>
        private bool Grayscale { get; }

        /// <summary>
        /// Returns option to generate low quality PDF (to reduce the result document size).
        /// </summary>
        private bool LowQuality { get; }

        /// <summary>
        /// Returns option to suppress wkhtmltopdf debug/info log messages.
        /// </summary>
        private bool Quiet { get; }

        /// <summary>
        /// Returns option that allows web pages to run JavaScript.
        /// </summary>
        private bool EnableJavaScript { get; }

        /// <summary>
        /// Returns delay for JavaScript finish (will applies only if JavaScript enabled).
        /// </summary>
        private TimeSpan JavaScriptDelay { get; }

        /// <summary>
        /// Returns option that allows make links to remote web pages.
        /// </summary>
        private bool EnableExternalLinks { get; }

        /// <summary>
        /// Returns option that allows to load or print images.
        /// </summary>
        private bool EnableImages { get; }

        /// <summary>
        /// Returns maximum execution time for PDF generation process (by default is null that means no timeout).
        /// </summary>
        public TimeSpan? ExecutionTimeout { get; }

        /// <summary>
        /// Compose all settings to single wkhtmltopdf command line arguments string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (PageSize != PageSize.Default)
                builder.AppendFormat(" -s {0}", PageSize.ToString("G"));
            if (Orientation != PageOrientation.Default)
                builder.AppendFormat(" -O {0}", Orientation.ToString("G"));
            if (Margins.Left.HasValue)
                builder.AppendFormat(CultureInfo.InvariantCulture, " -L {0}", Margins.Left.Value);
            if (Margins.Top.HasValue)
                builder.AppendFormat(CultureInfo.InvariantCulture, " -T {0}", Margins.Top.Value);
            if (Margins.Right.HasValue)
                builder.AppendFormat(CultureInfo.InvariantCulture, " -R {0}", Margins.Right.Value);
            if (Margins.Bottom.HasValue)
                builder.AppendFormat(CultureInfo.InvariantCulture, " -B {0}", Margins.Bottom.Value);
            if (Grayscale)
                builder.Append(" -g");
            if (LowQuality)
                builder.Append(" -l");
            if (Quiet)
                builder.Append(" -q");
            if (EnableJavaScript)
            {
                builder.Append(" --enable-javascript");
                var jsDelayString = JavaScriptDelay.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
                builder.Append($" --javascript-delay {jsDelayString}");
            }
            else
            {
                builder.Append(" --disable-javascript");
            }
            if (EnableExternalLinks)
            {
                builder.Append(" --enable-external-links");
            }
            else
            {
                builder.Append(" --disable-external-links");
            }
            if (EnableImages)
            {
                builder.Append(" --images");
            }
            else
            {
                builder.Append(" --no-images");
            }
            return builder.ToString().Trim();
        }
    }
}