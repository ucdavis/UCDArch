using UCDArch.Web.Controller;
using UCDArch.Web.Attributes;

namespace SampleUCDArchApp.Controllers
{
    [Version(MajorVersion = 3)]
    [ServiceMessage("SampleUcdArchApp", ViewDataKey = "ServiceMessages", MessageServiceAppSettingsKey = "MessageService")]
    public abstract class ApplicationController : SuperController
    {
    }
}