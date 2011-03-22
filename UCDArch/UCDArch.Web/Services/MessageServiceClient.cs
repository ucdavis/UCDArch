using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace UCDArch.Web.Services
{
    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public class MessageServiceClient : ClientBase<IMessageService>, IMessageService
    {
        public MessageServiceClient()
        {
        }

        public MessageServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public MessageServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public MessageServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public MessageServiceClient(Binding binding,
                                    EndpointAddress remoteAddress) :
                                        base(binding, remoteAddress)
        {
        }

        #region IMessageService Members

        public ServiceMessage[] GetMessages(string appName)
        {
            return base.Channel.GetMessages(appName);
        }

        public IAsyncResult BeginGetMessages(string appName, AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginGetMessages(appName, callback, asyncState);
        }

        public ServiceMessage[] EndGetMessages(IAsyncResult result)
        {
            return base.Channel.EndGetMessages(result);
        }

        #endregion
    }
}