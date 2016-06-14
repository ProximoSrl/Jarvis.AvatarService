using Jarvis.AvatarService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Jarvis.AvatarService.Support
{
    public class AuthController : ApiController
    {
        protected bool IsAuthenticated(HttpRequestHeaders headers)
        {
            var authorization = headers.Authorization;
            BasicAuthenticationIdentity identity = null;
            try
            {
                if (authorization.Scheme == "Basic")
                {
                    var parameters = authorization.Parameter;
                    var encodedParameters = Encoding.Default.GetString(Convert.FromBase64String(parameters));
                    var tokens = encodedParameters.Split(':');
                    identity = new BasicAuthenticationIdentity(tokens[0], tokens[1]);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (!String.Equals(identity.UserName, ConfigurationManager.AppSettings["user"]))
                throw new HttpRequestException("Utente non valido!");
            if (!String.Equals(identity.Password, ConfigurationManager.AppSettings["password"]))
                throw new HttpRequestException("Password non valida!");

            return true;
        }
    }
}