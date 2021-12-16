using System.IO;

namespace CoreHtmlToImage.Console
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			// From HTML string
			HtmlConverter converter = new();
			string html = "<div><strong>Hello</strong> World!</div>";
			byte[] htmlBytes = converter.FromHtmlString(html);

			// From URL
			byte[] urlBytes = converter.FromUrl("http://google.com", 800, format: ImageFormat.Png, quality: 90);
			File.WriteAllBytes("D:\\image.png", urlBytes);
		}
	}
}
