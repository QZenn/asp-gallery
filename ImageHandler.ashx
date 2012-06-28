<%@ WebHandler Language="C#" Class="Handlers.ImageHandler" %>

using System;
using System.Web;

namespace Handlers
{

    public class ImageHandler : IHttpHandler
    {
        private static int locl;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}