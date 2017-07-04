using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class ImageStreamResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            var Response = context.HttpContext.Response;
            Response.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            int value = 40;
            Response.ContentType = "image/png";
            Bitmap gif = new Bitmap(value, 18, PixelFormat.Format32bppArgb);
            Color c = ColorTranslator.FromHtml("#" + context.HttpContext.Request.QueryString["color"]);
            LinearGradientBrush b = new LinearGradientBrush(new PointF(0, 9), new PointF(value, 9),
                c, Color.FromArgb(0, Color.White));

            Graphics g = Graphics.FromImage(gif);
            g.Clear(Color.FromArgb(0, Color.White));
            g.FillRectangle(b, new Rectangle(0, 0, value, 18));
            g.Dispose();
            b.Dispose();
            MemoryStream ms = new MemoryStream();
            gif.Save(ms, ImageFormat.Png);
            gif.Dispose();
            ms.WriteTo(Response.OutputStream);
            Response.End();
        }
    }
}