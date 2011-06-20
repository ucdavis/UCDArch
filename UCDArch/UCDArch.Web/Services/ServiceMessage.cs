using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace UCDArch.Web.Services
{
    [DebuggerStepThrough]
    [DataContract(Name = "ServiceMessage", Namespace = "http://schemas.datacontract.org/2004/07/Catbert4.Services.Wcf")]
    [SerializableAttribute]
    public class ServiceMessage
    {
        [DataMember]
        public bool Critical { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public bool Global { get; set; }
    }
}