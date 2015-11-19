using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Jarvis.AvatarService.Controllers
{
    [RoutePrefix("api/avatar")]
    public class AvatarController : ApiController
    {
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            using (var rectangleFont = new Font("Arial", 36, FontStyle.Bold))
            using (var bitmap = new Bitmap(90, 90, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var backgroundColor = Color.DeepSkyBlue;
                g.Clear(backgroundColor);
                g.DrawString(id, rectangleFont, Brushes.Beige, new PointF(4, 16));

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ms.ToArray())
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                    return result;
                }
            }
        }

    }
}
