using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace SampleUCDArchApp.Controllers
{
    //TODO: Replace MessageServiceUrl with MessageServiceAppSettingsKey, and create an AppSettings key with the proper value
    [Version(MajorVersion = 3)]
    [ServiceMessage("SampleUcdArchApp", ViewDataKey = "ServiceMessages", MessageServiceUrl = "https://test.caes.ucdavis.edu/Catbert4/public/message.svc")]
    public abstract class ApplicationController : SuperController
    {
    }
}