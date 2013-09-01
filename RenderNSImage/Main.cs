using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.WebKit;
using System.IO;

namespace RenderNSImage
{
	//monomac, use a webview to render to image
	class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			var wv = new WebView();
			wv.Frame = new RectangleF(0, 0, 512, 512);
			wv.FinishedLoad += (object sender, WebFrameEventArgs e) => {
				var bitrep = new NSBitmapImageRep(IntPtr.Zero, (int)wv.Frame.Width, (int)wv.Frame.Height, 8, 4, true, false, NSColorSpace.CalibratedRGB, 4 * (int)wv.Frame.Width, 32);

				NSGraphicsContext.GlobalSaveGraphicsState();
				var gc = NSGraphicsContext.FromBitmap(bitrep);
				NSGraphicsContext.CurrentContext = gc;
				wv.DisplayRectIgnoringOpacity(wv.Bounds, gc);
				NSGraphicsContext.GlobalRestoreGraphicsState();

				var image = new NSImage(bitrep.Size);
				image.AddRepresentation(bitrep);

				var data = bitrep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Png, new NSDictionary());
				using(var f = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop/google.png"), FileMode.Create))
					data.AsStream().CopyTo(f);
				NSApplication.SharedApplication.Stop(wv);
			};
			var req = new NSUrlRequest(NSUrl.FromString("http://google.com"));
			wv.MainFrame.LoadRequest(req);
			NSApplication.Main (args);
		}
	}
}	

