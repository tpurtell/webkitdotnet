using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebKit;

//web kit on windows
// has a nasty 2x2 pixel display because it requires a window host that i didnt figure out how to hide
namespace RenderToImage
{
    public class HiddenBrowser : Form, IWebKitBrowserHost
    {
        public WebKitBrowser Browser { get; private set; }
        public HiddenBrowser()
        {
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            Text = string.Empty;
            ShowInTaskbar = false;
            Width = 0;
            Height = 0;
            MinimumSize = new Size(0, 0);
            Browser = new WebKitBrowser();
            Controls.Add(Browser);
            Browser.Width = 512;
            Browser.Height = 512;
        }

        bool IWebKitBrowserHost.InDesignMode
        {
            get { return LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
        }
    }
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length < 2)
                throw new Exception("missing args, need html file and png file");

            if(!File.Exists(args[0]))
                throw new Exception(string.Format("missing html file {0}", args[0]));

            if (Directory.Exists(args[1]))
                throw new Exception(string.Format("png file is a directory {0}", args[1]));

            var w = new HiddenBrowser();
            w.Browser.Url = new Uri(Path.GetFullPath(args[0]));
            w.Shown += (o, a) => { w.Size = new Size(0, 0); };
            //still shows as 2x2
            w.Show();
            
            w.Browser.DocumentCompleted += (sender, eventArgs) =>
            {
                var b = new Bitmap(w.Browser.Width, w.Browser.Height, PixelFormat.Format32bppArgb);
                w.Browser.DrawToBitmap(b, w.Browser.ClientRectangle);
                b.Save(args[1], ImageFormat.Png);
                Application.Exit();
            };
            Application.Run();
        }
    }
}
