﻿using System;
using System.IO;
using System.Web.Mvc;

namespace ahmadi.Infrastructure.Helper
{
    public static class CaptureHelper
    {
        public static string RenderViewToString(string viewPath, object model, ControllerContext context)
        {
            var viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;
            string result = String.Empty;
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                          context.Controller.ViewData,
                                          context.Controller.TempData,
                                          sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }
            return result;
        }
    }
}