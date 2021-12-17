using System;
using System.Diagnostics;
using System.IO;

namespace CoreHtmlToImage
{
	/// <summary>
	/// Html Converter. Converts HTML string and URLs to image bytes
	/// </summary>
	public class HtmlConverter
	{
		private const string ToolFilename = "wkhtmltoimage";
		private static readonly string Directory = AppContext.BaseDirectory;
		private static readonly string ToolFilepath;

		static HtmlConverter()
		{
			// Check on what platform we are
			if (OperatingSystem.IsWindows())
			{
				ToolFilepath = Path.Combine(Directory, ToolFilename + ".exe");
			}
			else if (OperatingSystem.IsLinux())
			{
				// Check if wkhtmltoimage package is installed on this distro in using which command
				Process process = Process.Start(new ProcessStartInfo
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					WorkingDirectory = "/bin/",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					FileName = "/bin/bash",
					Arguments = "which wkhtmltoimage",
				})!;

				string answer = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				if (!string.IsNullOrEmpty(answer) && answer.Contains("wkhtmltoimage"))
					ToolFilepath = ToolFilename;
				else
					throw new("wkhtmltoimage does not appear to be installed on this linux system according to 'which' command; go to https://wkhtmltopdf.org/downloads.html");
			}
			else
			{
				// OSX not implemented
				throw new("OSX Platform not supported yet");
			}
		}

		/// <summary>
		/// Converts HTML string to image
		/// </summary>
		/// <param name="html">HTML string</param>
		/// <param name="width">Output document width</param>
		/// <param name="format">Output image format</param>
		/// <param name="quality">Output image quality 1-100</param>
		/// <returns></returns>
		public byte[] FromHtmlString(string html, int width = 1024, ImageFormat format = ImageFormat.Jpg, int quality = 100)
		{
			string filename = Path.Combine(Directory, $"{Guid.NewGuid()}.html");
			File.WriteAllText(filename, html);
			byte[] bytes = FromUrl(filename, width, format, quality);
			File.Delete(filename);
			return bytes;
		}

		/// <summary>
		/// Converts HTML page to image
		/// </summary>
		/// <param name="url">Valid http(s):// URL</param>
		/// <param name="width">Output document width</param>
		/// <param name="format">Output image format</param>
		/// <param name="quality">Output image quality 1-100</param>
		/// <returns></returns>
		public byte[] FromUrl(string url, int width = 1024, ImageFormat format = ImageFormat.Jpg, int quality = 100)
		{
			string imageFormat = format.ToString().ToLower();
			string filename = Path.Combine(Directory, $"{Guid.NewGuid().ToString()}.{imageFormat}");

			string args;

			bool isLocalPath = !url.StartsWith("http") && new Uri(url).IsFile;
			if (isLocalPath)
				args = $"--quality {quality} --width {width} -f {imageFormat} \"{url}\" \"{filename}\"";
			else
				args = $"--quality {quality} --width {width} -f {imageFormat} {url} \"{filename}\"";

			Process process = Process.Start(new ProcessStartInfo(ToolFilepath, args)
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = Directory,
				RedirectStandardError = true,
			})!;

			process.ErrorDataReceived += (_, e) => throw new(e.Data);
			process.WaitForExit();

			if (!File.Exists(filename))
				throw new("Something went wrong. Please check input parameters");

			byte[] bytes = File.ReadAllBytes(filename);
			File.Delete(filename);
			return bytes;
		}
	}
}
