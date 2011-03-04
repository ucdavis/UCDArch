using System.Web;
using Telerik.Web.Mvc.UI;
using Telerik.Web.Mvc.UI.Fluent;
using UCDArch.Core.PersistanceSupport;

namespace SampleUCDArchApp.Helpers
{
    public class CustomGridBuilder<T> : GridBuilder<T>, IHtmlString where T : class
    {
        protected bool UseTransaction { get; set; }
        protected bool ShouldDisplayAlternateMessage { get; set; }
        protected string AlternateMessage { get; set; }

        public CustomGridBuilder(Grid<T> component)
            : base(component)
        {
            UseTransaction = false;
            ShouldDisplayAlternateMessage = false;
        }

        public GridBuilder<T> Transactional()
        {
            UseTransaction = true;

            return this;
        }

        public CustomGridBuilder<T> DisplayAlternateMessageWhen(bool condition, string message)
        {
            ShouldDisplayAlternateMessage = condition;
            AlternateMessage = message;

            return this;
        }

        public override void Render()
        {
            if (ShouldDisplayAlternateMessage)
            {
                Component.ViewContext.RequestContext.HttpContext.Response.Write(HttpUtility.HtmlEncode(AlternateMessage));

                return; //Don't do the normal render
            }

            if (UseTransaction)
            {
                using (var ts = new TransactionScope())
                {
                    base.Render();

                    ts.CommitTransaction();
                }
            }
            else
            {
                base.Render();
            }
        }
    }
}