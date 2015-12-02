using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Jarvis.AvatarService.Support;
using System.Configuration;
using System.IO;

namespace Jarvis.AvatarService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            InitializeAvatarBuilder();

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }

        private static void InitializeAvatarBuilder()
        {
            var rootFolder = ConfigurationManager.AppSettings["RootFolder"];
            if (String.IsNullOrEmpty(rootFolder))
            {
                AvatarBuilder.RootFolder = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            }
            else
            {
                AvatarBuilder.RootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootFolder);
            }
            if (!Directory.Exists(AvatarBuilder.RootFolder))
            {
                Directory.CreateDirectory(AvatarBuilder.RootFolder);
            }
        }
    }
}
