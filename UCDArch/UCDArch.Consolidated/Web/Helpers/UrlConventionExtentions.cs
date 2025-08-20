///Placing in Microsoft.AspNetCore.Mvc to augment UrlHelper and avoid having to chase references
namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlConventionExtentions
    {
        public static string Image(this IUrlHelper urlHelper, string imageName)
        {
            return urlHelper.Content(string.Format("~/Images/{0}", imageName));
        }

        public static string Css(this IUrlHelper urlHelper, string styleSheet)
        {
            return urlHelper.Content(string.Format("~/CSS/{0}", styleSheet));
        }

        public static string Script(this IUrlHelper urlHelper, string scriptName)
        {
            return urlHelper.Content(string.Format("~/Scripts/{0}", scriptName));
        }
    }
}