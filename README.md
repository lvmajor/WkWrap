# WkWrap
[wkhtmltopdf](http://wkhtmltopdf.org/) cross-platform C# wrapper for .NET Core.
## Overview
This project is build against [.NET Standard 1.4](https://docs.microsoft.com/en-us/dotnet/articles/standard/library) using the [RTM tooling](https://www.microsoft.com/net/download/core) that ships with [Visual Studio 2017](https://www.visualstudio.com/downloads/). This is the only configuration I'm support on the issue tracker. WkWrap calls wkhtmltopdf in stream-based processing mode, so you don't need to provide any temp folders.

## Getting started
### Installation
The current version of the library is [available on NuGet.](https://www.nuget.org/packages/WkWrap.Core)
```
Install-Package WkWrap.Core
```
### Simple usage
WkWrap is designed as easy to use wkhtmltopdf cross-platform wrapper. To start using it you need to [install wkhtmltopdf](http://wkhtmltopdf.org/downloads.html) on your system and create instance of *HtmlToPdfConverter* class. One thing you need to do is provide path to *wkhtmltopdf* executable file (may require chmod+x on \* nix based systems).
```csharp
var wkhtmltopdf = new FileInfo(@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe");
var converter = new HtmlToPdfConverter(wkhtmltopdf);
var pdfBytes = converter.ConvertToPdf(html);
```
### Customize processing
If you want to customize processing options you can do it with providing custom *ConversionSettings* class instance. 
```csharp
var wkhtmltopdf = new FileInfo(@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe");
var converter = new HtmlToPdfConverter(wkhtmltopdf);
var settings = new ConversionSettings(
    pageSize: PageSize.A3,
    orientation: PageOrientation.Landscape,
    margins: new PageMargins(0, 0, 0, 0),
    grayscale: true,
    lowQuality: true,
    quiet: true,
    enableJavaScript: true,
    javaScriptDelay: null,
    enableExternalLinks: true,
    enableImages: true,
    executionTimeout: null);
var pdfBytes = converter.ConvertToPdf(html, Encoding.UTF8, settings);
```
### Stream-based processing
If you need to convert html in stream mode you can do it with specific stream-based *HtmlToPdfConverter* instance method.
```csharp
var resultStream = new MemoryStream();
var wkhtmltopdf = new FileInfo(@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe");
var converter = new HtmlToPdfConverter(wkhtmltopdf);
converter.ConvertToPdf(htmlMemorySream, resultStream, ConversionSettings.Default());
```
### Custom wkhtmltopdf options
Instance of *ConversionSettings* just converts to wkhtmltopdf command line arguments with *.ToString()* method call. So if you want to specify custom wkhtmltopdf command line arguments you can do it too!
```csharp
var resultStream = new MemoryStream();
var wkhtmltopdf = new FileInfo(@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe");
var converter = new HtmlToPdfConverter(wkhtmltopdf);
converter.ConvertToPdf(htmlMemorySream, resultStream, "-s A3 -L 0 -T 0 -B 0 -R 0 -g -q");
```
Or you can call wkhtmltopdf with default options using
```csharp
var resultStream = new MemoryStream();
var wkhtmltopdf = new FileInfo(@"C:\Program Files\wkhtmltopdf\bin\wkhtmltopdf.exe");
var converter = new HtmlToPdfConverter(wkhtmltopdf);
string settings = null;
converter.ConvertToPdf(htmlMemorySream, resultStream, settings);
```