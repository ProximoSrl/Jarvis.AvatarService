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
using System.Web;

namespace Jarvis.AvatarService.Controllers
{
    [RoutePrefix("api/avatar")]
    [Authorize]
    public class AvatarController : ApiController
    {
        [HttpGet]
        [Route("{userId}")]
        [AllowAnonymous]
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
        public async Task<HttpResponseMessage> Post(String userId, int size)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("File non valido!"));

            try
            {
                var path = HttpContext.Current.Server.MapPath("~/App_Data/temp");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Prepara lo streaming dei dati sul file corretto
                MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(path);
                // Sovrascrive il file con il nuovo avatar
                await Request.Content.ReadAsMultipartAsync(provider);

                var fileData = provider.FileData.FirstOrDefault();
                if (fileData == null)
                    throw new Exception("File not found!");
                // Prendo il nome del file temporaneo
                var fileName = fileData.Headers.ContentDisposition.FileName.Trim(Path.GetInvalidFileNameChars());
                var localFileName = fileData.LocalFileName;

                // Scrivo il nuovo file
                using (var sr = File.OpenRead(localFileName))
                {
                    AvatarBuilder.CreateByStream(userId, size, sr);
                }

                // Elimino il file temp
                File.Delete(localFileName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
