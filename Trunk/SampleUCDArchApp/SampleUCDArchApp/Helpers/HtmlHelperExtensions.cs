using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace SampleUCDArchApp.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static CustomGridBuilder<T> Grid<T>(this HtmlHelper htmlHelper, IEnumerable<T> dataModel) where T : class
        {
            var builder = htmlHelper.Telerik().Grid(dataModel);

            return new CustomGridBuilder<T>(builder);
        }
    }
}