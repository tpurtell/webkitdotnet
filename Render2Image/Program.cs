using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//native webbrowser control
// ie on windows
// fail on mac (no gluezilla)
namespace RenderToImage
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new Exception("missing args, need html file and png file");

            if (Directory.Exists(args[1]))
                throw new Exception(string.Format("png file is a directory {0}", args[1]));

//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
            var wb = new WebBrowser();
            wb.Width = 512;
            wb.Height = 512;
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;
            var done = new ManualResetEvent(false);
            wb.DocumentCompleted += (sender, eventArgs) =>
            {
                var b = new Bitmap(wb.Width, wb.Height, PixelFormat.Format32bppArgb);
                wb.DrawToBitmap(b, wb.DisplayRectangle);
                b.Save(args[1], ImageFormat.Png);
                Application.Exit();
            };

            wb.Url = new Uri(args[0]);
            wb.Visible = true;
            Application.Run();
        }
    }
}
