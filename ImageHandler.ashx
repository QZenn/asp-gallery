<%@ WebHandler Language="C#" Class="Handlers.ImageHandler" %>

using System;
using System.Web;

namespace Handlers
{

    public class ImageHandler : IHttpHandler
    {
        private static int locl;
        private HttpContext context;

        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            string requestedImageName = context.Request["file"];
            //requestedImageName = requestedImageName.Substring(requestedImageName.LastIndexOf("\\") + 1).Trim();
            
            
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