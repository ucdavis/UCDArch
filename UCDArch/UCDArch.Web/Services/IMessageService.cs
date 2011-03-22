using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace UCDArch.Web.Services
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(Namespace = "https://secure.caes.ucdavis.edu/Catbert4",
        ConfigurationName = "IMessageService")]
    public interface IMessageService
    {
        [OperationContractAttribute(
            Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessages",
            ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessagesResponse")]
        ServiceMessage[] GetMessages(string appName);

        [OperationContractAttribute(AsyncPattern = true,
            Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessages",
            ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessagesResponse")]
        IAsyncResult BeginGetMessages(string appName, AsyncCallback callback, object asyncState);

        ServiceMessage[] EndGetMessages(IAsyncResult result);
    }
}