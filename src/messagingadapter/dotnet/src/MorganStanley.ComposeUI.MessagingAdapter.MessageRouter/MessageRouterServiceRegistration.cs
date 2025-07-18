using MorganStanley.ComposeUI.Messaging;

namespace MorganStanley.ComposeUI.MessagingAdapter.MessageRouter
{
    internal class MessageRouterServiceRegistration(string serviceName, IMessageRouter messageRouter) : IAsyncDisposable
    {
        private int _disposed = 0;
        private readonly string _serviceName = serviceName;
        private readonly IMessageRouter _messageRouter = messageRouter;


        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) > 0)
            {
                return;
            }
            await _messageRouter.UnregisterServiceAsync(_serviceName);
        }
    }
}
