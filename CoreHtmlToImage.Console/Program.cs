using System;
using System.IO;

namespace CoreHtmlToImage.Console
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			// From HTML string
			HtmlConverter converter = new();
			const string html = "<div><strong>Hello</strong> World!</div>";
			byte[] htmlBytes = converter.FromHtmlString(html);

			// From URL
			byte[] urlBytes = converter.FromUrl("http://google.com", 800, format: ImageFormat.Png, quality: 90);
			string path = Path.Combine(AppContext.BaseDirectory, "image.png");
			File.WriteAllBytes(path, urlBytes);
			System.Console.WriteLine($"Saved image to {path}");
		}
	}
}
