///Placing in System.Web.Mvc to augment UrlHelper and avoid having to chase references
namespace System.Web.Mvc
{
    public static class UrlConventionExtentions
    {
        public static string Image(this UrlHelper urlHelper, string imageName)
        {
            return urlHelper.Content(string.Format("~/Images/{0}", imageName));
        }

        public static string Css(this UrlHelper urlHelper, string styleSheet)
        {
            return urlHelper.Content(string.Format("~/CSS/{0}", styleSheet));
        }

        public static string Script(this UrlHelper urlHelper, string scriptName)
        {
            return urlHelper.Content(string.Format("~/Scripts/{0}", scriptName));
        }
    }
}