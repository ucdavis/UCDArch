using System;
using System.ServiceModel;

namespace UCDArch.Web.Services
{
    [ServiceContract(Namespace = "https://secure.caes.ucdavis.edu/Catbert4")]
    public interface IMessageService
    {
        [OperationContractAttribute(Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessages",
            ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessagesResponse")]
        ServiceMessage[] GetMessages(string appName);

        [OperationContractAttribute(AsyncPattern = true,
            Action = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessages",
            ReplyAction = "https://secure.caes.ucdavis.edu/Catbert4/IMessageService/GetMessagesResponse")]
        IAsyncResult BeginGetMessages(string appName, AsyncCallback callback, object asyncState);

        ServiceMessage[] EndGetMessages(IAsyncResult result);
    }
}