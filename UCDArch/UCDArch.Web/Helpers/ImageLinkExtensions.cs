using System.Collections.Generic;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class ImageLinkExtensions
    {
        public const string StrNullOrEmpty = "Value cannot be null or empty.";

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName)
        {
            return ImageLink(htmlHelper /* controllerName */, imageName, altText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, object routeValues)
        {
            return ImageLink(htmlHelper /* controllerName */, imageName, altText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, object routeValues, object linkHtmlAttributes, object imagelinkHtmlAttributes)
        {
            return ImageLink(htmlHelper /* controllerName */, imageName, altText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(linkHtmlAttributes), new RouteValueDictionary(imagelinkHtmlAttributes));
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, RouteValueDictionary routeValues)
        {
            return ImageLink(htmlHelper /* controllerName */, imageName, altText, actionName, null, routeValues, new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> linkHtmlAttributes, IDictionary<string, object> imagelinkHtmlAttributes)
        {
            return ImageLink(htmlHelper /* controllerName */, imageName, altText, actionName, null, routeValues, linkHtmlAttributes, imagelinkHtmlAttributes);
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, string controllerName)
        {
            return ImageLink(htmlHelper, imageName, altText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary(), new RouteValueDictionary());
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, string controllerName, object routeValues, object linkHtmlAttributes, object imagelinkHtmlAttributes)
        {
            return ImageLink(htmlHelper, imageName, altText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(linkHtmlAttributes), new RouteValueDictionary(imagelinkHtmlAttributes));
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> linkHtmlAttributes, IDictionary<string, object> imagelinkHtmlAttributes)
        {
            if (String.IsNullOrEmpty(altText))
            {
                throw new ArgumentException(StrNullOrEmpty, "altText");
            }
            return GenerateImageLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection /* routeName */, imageName, altText, null, actionName, controllerName, routeValues, linkHtmlAttributes, imagelinkHtmlAttributes);
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, string controllerName, string protocol, string hostName, string fragment, object routeValues, object linkHtmlAttributes, object imagelinkHtmlAttributes)
        {
            return ImageLink(htmlHelper, imageName, altText, actionName, controllerName, protocol, hostName, fragment, new RouteValueDictionary(routeValues), new RouteValueDictionary(linkHtmlAttributes), new RouteValueDictionary(imagelinkHtmlAttributes));
        }

        public static string ImageLink(this HtmlHelper htmlHelper, string imageName, string altText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> linkHtmlAttributes, IDictionary<string, object> imagelinkHtmlAttributes)
        {
            if (String.IsNullOrEmpty(altText))
            {
                throw new ArgumentException(StrNullOrEmpty, "altText");
            }

            return GenerateImageLink(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection /* routeName */, imageName, altText, null, actionName, controllerName, protocol, hostName, fragment, routeValues, linkHtmlAttributes, imagelinkHtmlAttributes);
        }

        public static string GenerateImageLink(RequestContext requestContext, RouteCollection routeCollection, string imageName, string altText, string routeName, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> linkHtmlAttributes, IDictionary<string, object> imagelinkHtmlAttributes)
        {
            return GenerateImageLink(requestContext, routeCollection /* protocol */ /* hostName */ /* fragment */, imageName, altText, routeName, actionName, controllerName, null, null, null, routeValues, linkHtmlAttributes, imagelinkHtmlAttributes);
        }

        public static string GenerateImageLink(RequestContext requestContext, RouteCollection routeCollection, string imageName, string altText, string routeName, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> linkHtmlAttributes, IDictionary<string, object> imagelinkHtmlAttributes)
        {
            return GenerateImageLinkInternal(requestContext, imageName, altText, actionName,
                                             controllerName, protocol, hostName, fragment, routeValues, linkHtmlAttributes,
                                             imagelinkHtmlAttributes
                /* includeImplicitMvcValues */);
        }

        private static string GenerateImageLinkInternal(RequestContext requestContext, string imageName, string altText, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> linkHtmlAttributes, IDictionary<string, object> imagelinkHtmlAttributes)
        {
            var urlHelper = new UrlHelper(requestContext);

            string url = urlHelper.Action(actionName, controllerName, routeValues, protocol, hostName);

            TagBuilder imageTagBuilder = new TagBuilder("img");
            imageTagBuilder.MergeAttribute("src", urlHelper.Image(imageName));
            imageTagBuilder.MergeAttribute("alt", altText);
            imageTagBuilder.MergeAttributes(imagelinkHtmlAttributes);

            TagBuilder tagBuilder = new TagBuilder("a")
            {
                InnerHtml = imageTagBuilder.ToString(TagRenderMode.SelfClosing)
            };

            tagBuilder.MergeAttributes(linkHtmlAttributes);
            tagBuilder.MergeAttribute("href", url);
            return tagBuilder.ToString(TagRenderMode.Normal);
        }
    }
}