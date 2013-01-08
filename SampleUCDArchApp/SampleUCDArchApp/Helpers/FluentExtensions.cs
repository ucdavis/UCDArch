using System.Collections.Generic;
using System.Web.Mvc;

namespace SampleUCDArchApp.Helpers
{
    public static class FluentHtmlExtensions
    {
        public static T IncludeUnobtrusiveValidationAttributes<T>(this T element, HtmlHelper htmlHelper) where T : MvcContrib.FluentHtml.Elements.IElement
        {
            IDictionary<string, object> validationAttributes = htmlHelper
                .GetUnobtrusiveValidationAttributes(element.GetAttr("name"));

            foreach (var validationAttribute in validationAttributes)
            {
                element.SetAttr(validationAttribute.Key, validationAttribute.Value);
            }

            return element;
        }
    }
}