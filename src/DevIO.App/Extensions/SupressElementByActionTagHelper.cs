﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;

namespace DevIO.App.Extensions
{
    [HtmlTargetElement("*", Attributes = "supress-by-action")]
    public class SupressElementByActionTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupressElementByActionTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("supress-by-action")]
        public string ActionName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var action = _httpContextAccessor.HttpContext.GetRouteData().Values["action"].ToString();

            if (ActionName.Contains(action)) return;

            output.SuppressOutput();
        }
    }
}
