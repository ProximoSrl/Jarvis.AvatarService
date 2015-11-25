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
using System.Threading.Tasks;

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

        [HttpPost]
        [Route("{userId}")]
        public async Task<HttpResponseMessage> Post(String userId, int size, string name)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("File non valido!"));

            try
            {
                var pathToFile = AvatarBuilder.CreateFor(userId, size, name);
                MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(pathToFile);
                await Request.Content.ReadAsMultipartAsync(provider);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
