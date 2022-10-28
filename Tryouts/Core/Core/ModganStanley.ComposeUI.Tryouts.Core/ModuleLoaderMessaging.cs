using MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;
using MorganStanley.ComposeUI.Tryouts.Messaging.Client;
using System.Reactive;
using System.Text.Json;

namespace ModganStanley.ComposeUI.Tryouts.Core
{
    internal class ModuleLoaderMessaging
    {
        private IMessageRouter _messageRouter;
        private IModuleLoader _moduleLoader;

        public ModuleLoaderMessaging(IMessageRouter messageRouter, IModuleLoader moduleLoader)
        {
            _messageRouter = messageRouter;
            _moduleLoader = moduleLoader;
        }

        public async Task Initialize()
        {
            var launchRequestObserver = Observer.Create<string?>(m =>
            {
                if (m == null)
                {
                    return;
                }
                LaunchRequest request = JsonSerializer.Deserialize<LaunchRequest>(m);
                _moduleLoader.RequestStartProcess(request);
            });
            await _messageRouter.SubscribeAsync("ComposeLaunchRequests", launchRequestObserver);

            var lifecycleEventObserver = Observer.Create<LifecycleEvent>(
                async e =>
                await _messageRouter.PublishAsync("ComposeLifecycleEvents", JsonSerializer.Serialize(e))
                );
            _moduleLoader.LifecycleEvents.Subscribe(lifecycleEventObserver);
        }
    }
}
