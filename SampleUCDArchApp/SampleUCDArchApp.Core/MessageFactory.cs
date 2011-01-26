
namespace SampleUCDArchApp.Core
{
    public interface IMessageFactory
    {
        void SendNewOrderMessage();
    }

    public class MessageFactory : IMessageFactory
    {
        public void SendNewOrderMessage()
        {
            //Send a new order message, maybe by email, web service, log, etc...
        }
    }
}