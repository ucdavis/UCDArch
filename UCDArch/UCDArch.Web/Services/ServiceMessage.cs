using System.Runtime.Serialization;

namespace UCDArch.Web.Services
{
    [DataContract]
    public class ServiceMessage
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public bool Critical { get; set; }
    }
}