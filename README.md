# WkWrap
wkhtmltopdf cross-platform C# wrapper for .NET Core.

## Overview ##

This project is against .NET Standard 1.4 using the RTM tooling that ships with Visual Studio 2017. This is the only configuration I'm support on the issue tracker.

## Getting started ##

WkWrap is designed as easy to use wkhtmltopdf cross-platform wrapper. To start using it you need to install wkhtmltopdf on your system and create instance of HtmlToPdfConverter class.
```csharp
var wkhtmltopdf = new FileInfo(@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe");
var converter = new HtmlToPdfConverter(wkhtmltopdf);
var pdfBytes = converter.ConvertToPdf(html);
```
One thing you need to do is provide path to wkhtmltopdf executable file (may require chmod+x on \* nix based systems).
WkWrap calls wkhtmltopdf in stream-based processing mode, so you don't need to provide any temp folders. If you want to customize processing options you can do it with providing custom ConversionSettings class instance. Or if you need to process html files in stream mode you can do it with specific stream-based HtmlToPdfConverter instance method.
