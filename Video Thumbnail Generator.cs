using NReco.VideoConverter;
using System.Drawing;
using System.Drawing.Drawing2D;

void Main()
{
string videoFolderPath = @"F:\Shows\Tv Shows\American Horror Story\Season 10 - [Double Feature]";

List<string> videoFormats = new List<string> {"avi","mp4","mkv"};
List<Bitmap> videoThumbnails = new List<Bitmap>();
foreach(string file in Directory.GetFiles(videoFolderPath))
{
		if (videoFormats.Contains ((Path.GetExtension (file).Substring (1))))
		{
			string showName = new DirectoryInfo (Path.GetDirectoryName (Path.Combine (file, @"..\..\"))).Name;
			string fileName = Path.GetFileNameWithoutExtension (file);
			Bitmap videoThumbnail = CreateThumbnail (file, 50, Color.FromArgb (50, 50, 50), showName, fileName, new Size (1920, 1080), 12);
			videoThumbnails.Add(videoThumbnail);
		}
}
Console.WriteLine(videoThumbnails);

}

Bitmap CreateThumbnail( string videoFilePath, int secondsIn,Color borderColor, string showLabel,string fileLabel,Size defaultSize,int percentResize)
{	
	string outputPath = Path.Combine (Path.GetDirectoryName(videoFilePath), Path.GetFileNameWithoutExtension (videoFilePath) + "_thumbnail.jpg");
	FFMpegConverter thumbnail = new FFMpegConverter();
	thumbnail.GetVideoThumbnail (videoFilePath, outputPath, secondsIn);
	Bitmap thumbnailBitmap = new Bitmap (outputPath);

	int height = (defaultSize.Height/100)*percentResize;
	int width = (defaultSize.Width/100)*percentResize;
	Bitmap thumbnailResized = new Bitmap (thumbnailBitmap, width, height);
	return DrawBitmapWithBorder(thumbnailResized,borderColor,showLabel,fileLabel);
}

private static Bitmap DrawBitmapWithBorder (Bitmap bmp, Color borderColor, string showLabel,string fileLabel)
{
	int borderSize = 5;
	int newWidth = bmp.Width + (borderSize * 2);
	int newHeight = bmp.Height + (borderSize * 2);

	Image newImage = new Bitmap (newWidth, newHeight);
	using (Graphics gfx = Graphics.FromImage (newImage))
	{
		using (Brush border = new SolidBrush (borderColor))
		{
			gfx.FillRectangle (border, 0, 0,newWidth, newHeight);
		}

		RectangleF rectf = new RectangleF(new PointF(0,0), new SizeF( bmp.Width, bmp.Height-5));
		RectangleF rectfShadow = new RectangleF(new PointF(0,3), new SizeF( bmp.Width, bmp.Height-5));
		RectangleF rectfName = new RectangleF(new PointF(0,8), new SizeF( bmp.Width, bmp.Height));
		RectangleF rectfNameShadow = new RectangleF(new PointF(0,11), new SizeF( bmp.Width, bmp.Height));
		StringFormat stringFormat = new StringFormat();
		
		gfx.DrawImage (bmp, new Rectangle (borderSize, borderSize, bmp.Width, bmp.Height));
		gfx.SmoothingMode = SmoothingMode.AntiAlias;
		gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
		gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
		
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Far;
		
		gfx.DrawString (showLabel, new Font ("Open Sans", 15,FontStyle.Bold),Brushes.Black,rectfShadow,stringFormat);
		gfx.DrawString (showLabel, new Font ("Open Sans", 15,FontStyle.Bold), Brushes.White,rectf,stringFormat);
		gfx.Flush();
		
		rectf = new RectangleF(new PointF(15,5), new SizeF( bmp.Width, bmp.Height));
		stringFormat.Alignment = StringAlignment.Far;
		stringFormat.LineAlignment = StringAlignment.Near;
		gfx.DrawString (fileLabel, new Font ("Open Sans", 15, FontStyle.Bold), Brushes.Black, rectfNameShadow, stringFormat);
		gfx.DrawString (fileLabel, new Font ("Open Sans", 15, FontStyle.Bold), Brushes.White, rectfName, stringFormat);
		gfx.Flush();
	}
	return (Bitmap)newImage;
}