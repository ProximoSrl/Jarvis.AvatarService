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
using Jarvis.AvatarService.Support;

namespace Jarvis.AvatarService.Controllers
{
    [RoutePrefix("api/avatar")]
    public class AvatarController : ApiController
    {
        [HttpGet]
        [Route("{userId}")]
        public HttpResponseMessage Get(string userId, int size, string name)
        {
            var pathToFile = AvatarBuilder.CreateFor(userId, size, name);

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(new FileStream(pathToFile, FileMode.Open));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;
        }
    }
}
